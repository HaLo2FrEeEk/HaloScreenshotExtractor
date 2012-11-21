using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

//
// Changes since last release:
//
// Read file name length (confirmShot())
// Added "Breakpoint", "Anchor 9", and "Tempest" mapIDs to the list
// Read timestamp from BLF instead of CON
// Fix "remove" and "save" bugs with adding single to list
// Prevent duplicate single entries (bug: dragging multiple files clears box)
// Improved update method (uses thread and XDocument)
// Added "Unearthed", "Condemned", and "Highlands" mapIDs to the list
//

namespace HaloScreenshots
{
    public partial class Form1 : Form
    {
        #region Global Variables
        Maps maps = new Maps();
        ScreenshotReader reader = new ScreenshotReader();
        Properties.Settings settings = Properties.Settings.Default;

        FileInfo finfo;
        DirectoryInfo dinfo;
        FileStream fs;
        BinaryReader br;

        List<int> gamesAllow = new List<int>() { 0x4D5307E6, 0x4D530877, 0x4D53885C, 0x4D53085B, 0x4D530919 }; // Halo 3, Halo 3: ODST, Halo Reach Beta, Halo: Reach
        string[] gamesTitles = new string[] { "Halo 3", "Halo 3: ODST", "Halo Reach Beta", "Halo: Reach", "Halo 4" };
        string[] gamesTitlesSafe = new string[] { "halo3", "odst", "reachbeta", "reach", "halo4" };
        int gameID;
        List<screenshotItem> screenshots = new List<screenshotItem>();

        string errorMsg;
        string errorTitle;
        int rightClickIndex;

        int hashTable;
        int contentStart = 0xC000;
        int fileListAdd;

        Version thisVer;
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetDoubleBuffered(fileList);
            thisVer = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += " " + thisVer.Major + "." + thisVer.Minor + "." + thisVer.Build;
#if DEBUG
            this.Text += " - DEBUG";
#endif

            submenuAutosave.Checked = settings.autosaveSingle;
        }

        #region Misc Methods
        /// <summary>
        /// Sets the supplied control to Double Buffered, reducing or eliminating flicker.
        /// </summary>
        /// <param name="control">Name of the control to make Double Buffered</param>
        private static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true });
        }

        /// <summary>
        /// Used for parsing FAT format date integers.
        /// </summary>
        /// <param name="xDateTime">Big Endian int32 read from the file list</param>
        /// <returns>DateTime representing the date the file was created</returns>
        private DateTime fatXDate(int xDateTime)
        {
            short xDate = (short)(xDateTime >> 16);
            short xTime = (short)(xDateTime & 0xFFFF);
            DateTime fileTime = new DateTime(
                    (((xDate & 0xFE00) >> 9) + 0x7BC),
                    ((xDate & 0x1E0) >> 5),
                    (xDate & 0x1F),
                    ((xTime & 0xF800) >> 11),
                    ((xTime & 0x7E0) >> 5),
                    ((xTime & 0x1F) * 2));

            return fileTime;
        }

        /// <summary>
        /// Returns a Unix timestamp
        /// </summary>
        /// <returns>int32 representing the number of seconds since the Unix Epoch</returns>
        public long UnixTimeNow()
        {
            TimeSpan _ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)_ts.TotalSeconds;
        }

        /// <summary>
        /// Returns the time calculated from a Unix timestamp
        /// </summary>
        /// <param name="timestamp">A Unix timestamp</param>
        /// <returns>DateTime object with the calculate time</returns>
        public DateTime TimestampToTime(int timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
        }
        #endregion

        #region Confirm Methods
        /// <summary>
        /// Confirms that the opened file is actually a CON file and not a LIVE, PIRS, or any other type of file, and that it is from one of the supported games.
        /// </summary>
        /// <returns>True or False depending on if the file is a CON or not</returns>
        private bool CONfirm()
        {
            fs = finfo.OpenRead();
            br = new BinaryReader(fs);
            if (br.BaseStream.Length < 4)
            {
                errorMsg = "This file is too small to be a valid CON file";
                errorTitle = "Not a CON file";
                return false;
            }
            int headerMagic = br.ReadBigInt32();
            if (headerMagic != 1129270816)
            {
                errorMsg = "This isn't a valid CON file, I can't continue processing it.  Sorry.";
                errorTitle = "Not a CON file";
                return false; // The file isn't a valid CON file, exit the function
            }
            br.BaseStream.Position = 0x360;
            int titleID = br.ReadBigInt32();
            if (!gamesAllow.Contains(titleID))
            {
                errorMsg = "Sorry, this game isn't supported.  Only Halo 3, Halo 3: ODST, Halo: Reach Beta, and Halo: Reach are supported.";
                errorTitle = "Game not supported";
                return false; // This isn't one of the 3 supported games, exit the function
            }
            gameID = gamesAllow.IndexOf(titleID);
            br.BaseStream.Position = 0x37B;
            hashTable = (((br.ReadByte() >> 1) & 1) == 0) ? 0xA000 : 0xB000;
            return true; // The file passed all the tests, CONfirmed.
        }

        /// <summary>
        /// Confirms that the opened file is actually a screenshot and not, say, a usermap or film.
        /// </summary>
        /// <returns>True or False depending on if the file is a screenshot CON of not</returns>
        private bool confirmShot()
        {
            fileListAdd = 0;
            br.BaseStream.Position = 0x37E;
            fileListAdd = (br.ReadInt24() * 4096); // Now we have the correct offset of our file list

            br.BaseStream.Position = contentStart + fileListAdd + 40;
            byte fileNameLength = (byte)(br.ReadByte() & 63);
            br.BaseStream.Position = contentStart + fileListAdd;
            string fileName = Encoding.ASCII.GetString(br.ReadBytes(fileNameLength));
            if (fileName != "screen.shot")
            {
                errorMsg = "Sorry, this doesn't look like a valid screenshot from a supported game.";
                return false; // The file list didn't report that the filename was screen.shot, this isn't a screenshot CON 
            }
            return true; // YAY!
        }
        #endregion

        #region Processing Methods
        /// <summary>
        /// Handles the extraction of a single screenshot from a supplied screenshot CON file
        /// </summary>
        /// <param name="savingFile">Bool determing whether the file will be automatically saved or not</param>
        /// <param name="warn">Bool determining whether the user will be warned of errors</param>
        private void processSingle(bool savingFile, bool warn)
        {
            if (!CONfirm())
            {
                if (warn)
                    errorShow();
                return;
            }
            if (!confirmShot())
            {
                if (warn)
                    errorShow();
                return;
            }

            // Begin reading the CON file list
            br.BaseStream.Position = contentStart + fileListAdd + 41;
            int numBlocks = br.ReadInt24();
            br.BaseStream.Position = contentStart + fileListAdd + 47;
            int fileStart = contentStart + (br.ReadInt24() * 4096); // I now know where my blf file starts, I can use this as a baseline to iterate through the various blf blocks.
            br.BaseStream.Position = contentStart + fileListAdd + 52;
            int fileSize = br.ReadBigInt32(); // And I also know how long my blf file is, but I don't think I'll need this.
            br.BaseStream.Position = contentStart + fileListAdd + 56;
            int fileTime = br.ReadBigInt32(); // FAT formatted time

            screenshotItem screenshot = new screenshotItem() // We need to set a few initial variables
            {
                fileName = finfo.FullName,
                //fileTime = fileTime,
                gameID = gameID,
                hashTable = hashTable,
                numBlocks = numBlocks
            };

            switch (gameID)
            {
                case 0: // Halo 3
                    screenshot = reader.shotHalo3(br, fileStart, screenshot);
                    break;
                case 1: // Halo 3: ODST
                    screenshot = reader.shotHalo3ODST(br, fileStart, screenshot);
                    break;
                case 2: // Halo: Reach Beta
                    screenshot = reader.shotReachBeta(br, fileStart, screenshot);
                    break;
                case 3: // Halo: Reach
                case 4: // Halo 4 (this is a test)
                    screenshot = reader.shotReach(br, fileStart, screenshot);
                    break;
            }

            if (savingFile)
            {
                sfd.FileName = screenshot.shotTitle;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    saveSingle(screenshot, sfd.FileName);
                    toolStripStatusLabel1.Text = "Saved " + sfd.FileName;
                }
                br.Close();
                fs.Close();
                return;
            }

            if(!screenshots.Contains(screenshot))
                screenshots.Add(screenshot);
            br.Close();
            fs.Close();
        }

        /// <summary>
        /// First-pass processing on multiple files (Batch Mode).  Hands the data off to processSingle() to finish.
        /// </summary>
        private void processMulti()
        {
#if DEBUG // TimeSpan stuff
            DateTime tstart = DateTime.Now;
#endif
            FileInfo[] temp_finfos = dinfo.GetFiles();
            if (temp_finfos.Length > 0)
            {
                screenshots.Clear();
                toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                foreach (FileInfo temp_finfo in temp_finfos)
                {
                    finfo = new FileInfo(temp_finfo.FullName);
                    processSingle(false, false);
                    Application.DoEvents();
                }

                if (screenshots.Count == 0)
                {
                    toolStripStatusLabel1.Text = "No screenshot files were found.";
                    toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    if (fileList.Items.Count != 0)
                    {
                        submenuSaveAll.Enabled = true;
                        submenuSaveSelected.Enabled = true;
                        submenuRemoveSelection.Enabled = true;
                        submenuRemoveInverseSelection.Enabled = true;
                    }
                    return;
                }

                toolStripStatusLabel1.Text = "Found " + screenshots.Count + " screenshot files";

                displayList();
                toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                submenuSaveAll.Enabled = true;
                submenuSaveSelected.Enabled = true;
                submenuRemoveSelection.Enabled = true;
                submenuRemoveInverseSelection.Enabled = true;
            }
            else
            {
                toolStripStatusLabel1.Text = "No screenshots found in " + dinfo.FullName;
                if (fileList.Items.Count != 0)
                {
                    submenuSaveAll.Enabled = true;
                    submenuSaveSelected.Enabled = true;
                    submenuRemoveSelection.Enabled = true;
                    submenuRemoveInverseSelection.Enabled = true;
                }
            }
#if DEBUG // TimeSpan stuff
            DateTime tend = DateTime.Now;
            TimeSpan duration = tend - tstart;
            string hours = (duration.Hours != 0) ? duration.Hours + " hours," : "";
            string secs = (duration.Minutes != 0) ? duration.Minutes + " minutes," : "";
            string mins = (duration.Seconds != 0) ? duration.Seconds + " seconds," : "";
            string mill = (duration.Milliseconds != 0) ? duration.Milliseconds + " ms" : "";
            toolStripStatusLabel1.Text += " Total Time: " + hours + mins + secs + mill;
#endif
        }

        /// <summary>
        /// Overload method for handling multiple files opened via Drag n' Drop 
        /// </summary>
        /// <param name="temp_finfos">Temporary FileInfo list of the batch-opened files</param>
        private void processMulti(List<FileInfo> temp_finfos)
        {
#if DEBUG // TimeSpan stuff
            DateTime tstart = DateTime.Now;
#endif
            if (temp_finfos.Count > 0)
            {
                //fileList.Items.Clear();
                screenshots.Clear();
                toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

                foreach (FileInfo temp_finfo in temp_finfos)
                {
                    finfo = new FileInfo(temp_finfo.FullName);
                    processSingle(false, false);
                    Application.DoEvents();
                }

                if (screenshots.Count == 0)
                {
                    toolStripStatusLabel1.Text = "No screenshot files were dropped.";
                    toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    if (fileList.Items.Count != 0)
                    {
                        submenuSaveAll.Enabled = true;
                        submenuSaveSelected.Enabled = true;
                        submenuRemoveSelection.Enabled = true;
                        submenuRemoveInverseSelection.Enabled = true;
                    }
                    return;
                }

                toolStripStatusLabel1.Text = "Dropped " + screenshots.Count + " screenshot files";

                displayList();
                toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                submenuSaveAll.Enabled = true;
                submenuSaveSelected.Enabled = true;
                submenuRemoveSelection.Enabled = true;
                submenuRemoveInverseSelection.Enabled = true;
            }
            else
            {
                toolStripStatusLabel1.Text = "No screenshot files were dropped.";
                if (fileList.Items.Count != 0)
                {
                    submenuSaveAll.Enabled = true;
                    submenuSaveSelected.Enabled = true;
                    submenuRemoveSelection.Enabled = true;
                    submenuRemoveInverseSelection.Enabled = true;
                }
            }
#if DEBUG // TimeSpan stuff
            DateTime tend = DateTime.Now;
            TimeSpan duration = tend - tstart;
            string hours = (duration.Hours != 0) ? duration.Hours + " hours," : "";
            string secs = (duration.Minutes != 0) ? duration.Minutes + " minutes," : "";
            string mins = (duration.Seconds != 0) ? duration.Seconds + " seconds," : "";
            string mill = (duration.Milliseconds != 0) ? duration.Milliseconds + " ms" : "";
            toolStripStatusLabel1.Text += " Total Time: " + hours + mins + secs + mill;
#endif
        }
        #endregion

        #region Saving Methods
        /// <summary>
        /// First-pass processing on saving all files (Batch Mode).  Hands the data off to saveSingle() to finish.
        /// </summary>
        /// <param name="path"></param>
        private void saveAll(string path)
        {
#if DEBUG // TimeSpan stuff
            DateTime tstart = DateTime.Now;
#endif
            int index = 1;
            int numFiles = screenshots.Count;
            int padLength = numFiles.ToString().Length;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = numFiles;

            foreach (screenshotItem shot in screenshots)
            {
                string savePath = path + "\\" + gamesTitlesSafe[shot.gameID] + "_" + (index).ToString().PadLeft(padLength, '0') + "_" + shot.shotTitle + ".jpg";
                toolStripStatusLabel1.Text = "Saving " + savePath;
                saveSingle(shot, savePath);
                toolStripProgressBar1.PerformStep();
                index++;
                Application.DoEvents();
            }

            toolStripStatusLabel1.Text = "Saved " + numFiles + " screenshots to " + path;
            settings.fbdSavePath = path;
            settings.Save();
#if DEBUG // TimeSpan stuff
            DateTime tend = DateTime.Now;
            TimeSpan duration = tend - tstart;
            string hours = (duration.Hours != 0) ? duration.Hours + " hours," : "";
            string secs = (duration.Minutes != 0) ? duration.Minutes + " minutes," : "";
            string mins = (duration.Seconds != 0) ? duration.Seconds + " seconds," : "";
            string mill = (duration.Milliseconds != 0) ? duration.Milliseconds + " ms" : "";
            toolStripStatusLabel1.Text += " Total Time: " + hours + mins + secs + mill;
#endif
        }

        /// <summary>
        /// First-pass processing on saving selected files (Batch Mode).  Hands the data off to saveSingle() to finish.
        /// </summary>
        /// <param name="p"></param>
        private void saveSelected(string path)
        {
#if DEBUG // TimeSpan stuff
            DateTime tstart = DateTime.Now;
#endif
            int numFiles = fileList.SelectedItems.Count;
            int padLength = numFiles.ToString().Length;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = numFiles;

            foreach (int index in fileList.SelectedIndices)
            {
                screenshotItem shot = screenshots[index];
                string savePath = path + "\\" + gamesTitlesSafe[shot.gameID] + "_" + (index).ToString().PadLeft(padLength, '0') + "_" + shot.shotTitle + ".jpg";
                toolStripStatusLabel1.Text = "Saving " + savePath;
                saveSingle(shot, savePath);
                toolStripProgressBar1.PerformStep();
                Application.DoEvents();
            }

            toolStripStatusLabel1.Text = "Saved " + numFiles + " screenshots to " + path;
            settings.fbdSavePath = path;
            settings.Save();
#if DEBUG // TimeSpan stuff
            DateTime tend = DateTime.Now;
            TimeSpan duration = tend - tstart;
            string hours = (duration.Hours != 0) ? duration.Hours + " hours," : "";
            string secs = (duration.Minutes != 0) ? duration.Minutes + " minutes," : "";
            string mins = (duration.Seconds != 0) ? duration.Seconds + " seconds," : "";
            string mill = (duration.Milliseconds != 0) ? duration.Milliseconds + " ms" : "";
            toolStripStatusLabel1.Text += " Total Time: " + hours + mins + secs + mill;
#endif
        }

        /// <summary>
        /// Extracts and saves a single screenshot supplied by saveMulti()
        /// </summary>
        /// <param name="shot">screenshotItem containing information about the screenshot to be saved</param>
        /// <param name="savePath">The path on the harddrive where the screenshot will be saved</param>
        private void saveSingle(screenshotItem shot, string savePath)
        {
            FileStream fSave = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fSave);

            bw.Write(reader.readFile(shot));

            bw.Close();
            fSave.Close();
        }
        #endregion

        /// <summary>
        /// Sorts the batch list of screenshots and displays them in the ListView fileList
        /// </summary>
        private void displayList()
        {
            screenshots = screenshots.OrderBy(x => x.fileTime).ToList();
            fileList.Items.Clear();

            foreach (screenshotItem shot in screenshots)
            {
                string fileName = shot.fileName.Remove(0, shot.fileName.LastIndexOf(@"\") + 1);
                ListViewItem lvi = new ListViewItem(fileName);
                lvi.SubItems.Add(shot.shotTitle);
                lvi.SubItems.Add(gamesTitles[shot.gameID]);
                lvi.SubItems.Add(maps.getMapName(shot.mapID));
                lvi.SubItems.Add("0x" + shot.jpegOffset.ToString("X"));
                lvi.SubItems.Add(shot.jpegLength.ToString());
                //lvi.SubItems.Add(fatXDate(shot.fileTime).ToString());
                lvi.SubItems.Add(TimestampToTime(shot.fileTime).ToString());
                fileList.Items.Add(lvi);
            }

            fileList.BeginUpdate();
            foreach (ColumnHeader col in fileList.Columns)
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            fileList.EndUpdate();

            submenuRemoveSelection.Enabled =
            submenuRemoveInverseSelection.Enabled =
            submenuRemoveAll.Enabled =
            submenuSaveAll.Enabled =
            submenuSaveSelected.Enabled =
                true;

        }

        /// <summary>
        /// Sends all the relevant data to Form previewForm for generating a preview pop up.
        /// </summary>
        /// <param name="index">The 0-based index of the selected screenshot in the sorted screenshot List.</param>
        private void getPreview(int index)
        {
            if (!File.Exists(screenshots[index].fileName))
            {
                errorMsg = "This file doesn't seem to exist...maybe you deleted it?";
                errorTitle = "Where'd it go?";
                errorShow();
                return;
            }
            previewForm previewForm = new previewForm();

            previewForm.showPreview(screenshots[index]);
            previewForm.ShowDialog();
        }

        /// <summary>
        /// Method to remove selected item(s) from the List and ListView
        /// </summary>
        private void removeFromList(bool inverse, bool all)
        {
            if (all)
            {
                fileList.Items.Clear();
                screenshots.Clear();
            }
            else
            {
                if (!inverse)
                {
                    while (fileList.SelectedItems.Count > 0)
                    {
                        int index = fileList.SelectedItems[0].Index;
                        fileList.Items.Remove(fileList.SelectedItems[0]);
                        screenshots.Remove(screenshots[index]);
                    }
                }
                else
                {
                    int item = 0;
                    while (fileList.Items.Count > fileList.SelectedItems.Count)
                    {
                        if (!fileList.Items[item].Selected)
                        {
                            screenshots.Remove(screenshots[fileList.Items[item].Index]);
                            fileList.Items.Remove(fileList.Items[item]);
                        }
                        else
                        {
                            item++;
                        }
                    }
                }
            }

            if (fileList.Items.Count == 0)
            {
                submenuRemoveSelection.Enabled =
                submenuRemoveInverseSelection.Enabled =
                submenuRemoveAll.Enabled =
                submenuSaveAll.Enabled =
                submenuSaveSelected.Enabled = false;
            }

            toolStripStatusLabel1.Text = "Displaying " + fileList.Items.Count + " screenshots";
        }

        /// <summary>
        /// Checks update.xml for any updates to the program.  Needs work...
        /// </summary>
        private void checkForUpdate(Dictionary<string, string> update)
        {
            toolStripStatusLabel1.Text = "Checking for updates...";
            Application.DoEvents();
            #region old code
            //HttpWebRequest updateRequest = (HttpWebRequest)WebRequest.Create("http://infectionist.com/extras/csharp/haloscreenshots/update.xml");
            //updateRequest.Method = "GET";
            //HttpWebResponse updateResponse = (HttpWebResponse)updateRequest.GetResponse();
            //if (updateResponse.StatusCode == HttpStatusCode.NotFound)
            //{
            //    MessageBox.Show("It appears that the update server might be temporarily unavailable.  Please try again later.");
            //    return;
            //}
            //StreamReader responseStream = new StreamReader(updateResponse.GetResponseStream());
            //string updateXmlString = responseStream.ReadToEnd();
            //XmlDocument updateXml = new XmlDocument();
            //updateXml.LoadXml(updateXmlString);
            //XmlNodeList updateInfo = updateXml.DocumentElement.ChildNodes;
            //Version updateVer = new Version(Convert.ToInt32(updateInfo[0].InnerText), Convert.ToInt32(updateInfo[1].InnerText), Convert.ToInt32(updateInfo[2].InnerText), 0);
            //string updateDetails = updateInfo[3].InnerText;
            //string updateDownload = updateInfo[4].InnerText;
            #endregion
            Version updateVer = new Version(Convert.ToInt32(update["currentMajorVersion"]),
                                            Convert.ToInt32(update["currentMinorVersion"]),
                                            Convert.ToInt32(update["currentRevision"]), 0);
            if (updateVer > thisVer)
            {
                DialogResult doUpdate = MessageBox.Show("Your version: " + thisVer.Major + "." + thisVer.Minor + "." + thisVer.Build + Environment.NewLine +
                    "Update Version: " + updateVer.Major + "." + updateVer.Minor + "." + updateVer.Build + Environment.NewLine + Environment.NewLine +
                    update["versionDetails"] + Environment.NewLine + Environment.NewLine +
                    "Would you like to download the update?", "An update is available!", MessageBoxButtons.YesNo);
                if (doUpdate == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(update["download"]);
                    toolStripStatusLabel1.Text = "Update complete";
                }
                else if (doUpdate == DialogResult.No)
                {
                    toolStripStatusLabel1.Text = "Not updated (Reason: User-canceled)";
                }
            }
            else
            {
                MessageBox.Show("You are using the most current version");
                toolStripStatusLabel1.Text = "Not updated (Reason: Already up to date)";
            }
            //updateResponse.Close();
            //responseStream.Close();
        }

        /// <summary>
        /// Simple error-handling method, simply displays the supplied error message with an exclamation icon
        /// </summary>
        private void errorShow()
        {
            MessageBox.Show(errorMsg, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region Event Handlers
        private void submenuSingle_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                finfo = new FileInfo(ofd.FileName);
                processSingle(settings.autosaveSingle, true);
                if (!settings.autosaveSingle)
                {
                    displayList();
                }
            }
        }

        private void submenuBatch_Click(object sender, EventArgs e)
        {
            fbd.ShowNewFolderButton = false;
            fbd.SelectedPath = settings.fbdPath;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                submenuSaveAll.Enabled = false;
                submenuSaveSelected.Enabled = false;
                submenuRemoveSelection.Enabled = false;
                submenuRemoveInverseSelection.Enabled = false;
                fbd.ShowNewFolderButton = false;
                settings.fbdPath = fbd.SelectedPath;
                settings.Save();
                dinfo = new DirectoryInfo(fbd.SelectedPath);
                processMulti();
            }
        }
        
        private void submenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submenuSaveAll_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(settings.fbdSavePath))
            {
                fbd.SelectedPath = settings.fbdPath;
            }
            else
            {
                fbd.SelectedPath = settings.fbdSavePath;
            }
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                saveAll(fbd.SelectedPath);
                settings.fbdSavePath = fbd.SelectedPath;
                settings.Save();
            }
        }

        private void submenuSaveSelected_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedItems.Count == 0)
            {
                errorMsg = "No files selected.";
                errorShow();
                return;
            }
            if (!Directory.Exists(settings.fbdSavePath))
            {
                fbd.SelectedPath = settings.fbdPath;
            }
            else
            {
                fbd.SelectedPath = settings.fbdSavePath;
            }
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                saveSelected(fbd.SelectedPath);
            }
        }

        private void submenuAbout_Click(object sender, EventArgs e)
        {
            // TODO: Improve this section, include credits
            MessageBox.Show("Created by HaLo2FrEeEk");
        }

        private void submenuUpdate_Click(object sender, EventArgs e)
        {
            settings.lastUpdate = UnixTimeNow();
            settings.Save();
            //checkForUpdate();
            bgw.RunWorkerAsync();
        }

        private void fileList_ItemActivate(object sender, EventArgs e)
        {
            int index = fileList.Items.IndexOf(fileList.SelectedItems[0]);
            getPreview(index);
        }

        private void fileList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (fileList.SelectedItems.Count > 1)
                {
                    contextMenuSaveSelected.Visible = true;
                    contextMenuSaveThis.Visible = false;
                }
                else
                {
                    contextMenuSaveSelected.Visible = false;
                    contextMenuSaveThis.Visible = true;
                    rightClickIndex = fileList.GetItemAt(e.X, e.Y).Index;
                }

                contextMenu1.Show(fileList, e.Location);
            }
        }

        private void contextMenuSaveThis_Click(object sender, EventArgs e)
        {
            screenshotItem shot = screenshots[rightClickIndex];
            sfd.FileName = shot.shotTitle;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                saveSingle(shot, sfd.FileName);
                toolStripStatusLabel1.Text = "Saved " + sfd.FileName;
            }
        }

        private void contextMenuSaveSelected_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(settings.fbdSavePath))
            {
                fbd.SelectedPath = settings.fbdPath;
            }
            else
            {
                fbd.SelectedPath = settings.fbdSavePath;
            }
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                saveSelected(fbd.SelectedPath);
                settings.fbdSavePath = fbd.SelectedPath;
                settings.Save();
            }
        }

        private void contextMenuRemove_Click(object sender, EventArgs e)
        {
            removeFromList(false, false);
        }

        private void submenuRemoveSelection_Click(object sender, EventArgs e)
        {
            removeFromList(false, false);
        }

        private void submenuRemoveInverseSelection_Click(object sender, EventArgs e)
        {
            removeFromList(true, false);
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeFromList(false, true);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData("FileDrop", false);
            if (fileList.Length > 1)
            {
                List<FileInfo> finfos = new List<FileInfo>();
                // It's a list of files, confirm that each is a file (not a directory), then add each to the list
                foreach (string filename in fileList)
                {
                    // We'll need some code to ignore directories dropped, only files for now
                    if ((File.GetAttributes(filename) & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        // It's a file, add it to the list
                        FileInfo finfo = new FileInfo(filename);
                        finfos.Add(finfo);
                    }
                }
                processMulti(finfos);
            }
            else
            {
                // It's either a single file or a directory
                string filename = fileList[0]; // Since we know it's only 1 file, we can store it in a string variable
                if ((File.GetAttributes(filename) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    // It's a directory
                    settings.fbdPath = filename;
                    settings.Save();
                    dinfo = new DirectoryInfo(filename);
                    processMulti();
                }
                else
                {
                    // It's a single file
                    finfo = new FileInfo(filename);
                    processSingle(settings.autosaveSingle, true);
                    if (!settings.autosaveSingle)
                    {
                        displayList();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            long diff = UnixTimeNow() - settings.lastUpdate;
            if (diff > (60 * 60 * 24 * 7))
            {
                DialogResult askUpdate = MessageBox.Show("It looks like it's been more than a week since you've checked for an update, would you like to check now?", "Update?", MessageBoxButtons.YesNo);
                if (askUpdate == DialogResult.Yes)
                {
                    //checkForUpdate();
                    bgw.RunWorkerAsync();
                }
                settings.lastUpdate = UnixTimeNow();
                settings.Save();
            }
        }

        private void submenuAutosave_Click(object sender, EventArgs e)
        {
            submenuAutosave.Checked = (submenuAutosave.Checked) ? false : true;
            settings.autosaveSingle = submenuAutosave.Checked;
            settings.Save();
        }

        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            XDocument doc = XDocument.Load("http://infectionist.com/extras/csharp/haloscreenshots/update.xml");
            e.Result = doc;
        }

        private void bgw_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Dictionary<string, string> update = new Dictionary<string, string>();
            foreach (XElement element in ((XDocument)e.Result).Elements("updateInfo").Elements())
            {
                update.Add(element.Name.LocalName, element.Value);
            }

            checkForUpdate(update);
        }
        #endregion
    }

    /// <summary>
    /// Allows me to better organize the data I read from a screenshot CON and store it all in a single List.
    /// </summary>
    public class screenshotItem : IEquatable<screenshotItem>
    {
        public string fileName { get; set; }
        public string shotTitle { get; set; }
        public int gameID { get; set; }
        public int mapID { get; set; }
        public int blfVersion { get; set; }
        public int hashTable { get; set; }
        public int numBlocks { get; set; }
        public int headerStart { get; set; }
        public int headerLength { get; set; }
        public int jpegOffset { get; set; }
        public int jpegLength { get; set; }
        public int fileTime { get; set; }

        public override string ToString()
        {
            return "fileName: " + this.fileName + ", " + Environment.NewLine +
                   "shotTitle: " + this.shotTitle + ", " + Environment.NewLine +
                   "gameID: " + this.gameID + ", " + Environment.NewLine +
                   "mapID: " + this.mapID + ", " + Environment.NewLine +
                   "blfVersion: " + this.blfVersion + ", " + Environment.NewLine +
                   "hashTable: " + this.hashTable + ", " + Environment.NewLine +
                   "numBlocks: " + this.numBlocks + ", " + Environment.NewLine +
                   "headerStart: " + this.headerStart + ", " + Environment.NewLine +
                   "headerLength: " + this.headerLength + ", " + Environment.NewLine +
                   "jpegOffset: " + this.jpegOffset + ", " + Environment.NewLine +
                   "jpegLength: " + this.jpegLength + ", " + Environment.NewLine +
                   "fileTime: " + this.fileTime;
        }

        #region IEquatable<screenshotItem> Members

        public bool Equals(screenshotItem other)
        {
            if (this.fileName.Equals(other.fileName) &&
                this.shotTitle.Equals(other.shotTitle) &&
                this.gameID.Equals(other.gameID) &&
                this.mapID.Equals(other.mapID) &&
                this.blfVersion.Equals(other.blfVersion) &&
                this.hashTable.Equals(other.hashTable) &&
                this.numBlocks.Equals(other.numBlocks) &&
                this.headerStart.Equals(other.headerStart) &&
                this.headerLength.Equals(other.headerLength) &&
                this.jpegOffset.Equals(other.jpegOffset) &&
                this.jpegLength.Equals(other.jpegLength) &&
                this.fileTime.Equals(other.fileTime))
                return true;
            else
                return false;
        }

        #endregion
    }
}