using System;
using System.IO;
using System.Text;

namespace HaloScreenshots
{
    class ScreenshotReader
    {
        public byte[] readFile(screenshotItem shot)
        {
#if DEBUG
            FileStream debug = new FileStream(shot.fileName + "_debug.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(debug);
            sw.WriteLine("Total Filesize: " + shot.jpegLength + ", numBlocks: " + shot.numBlocks + " hashBlock: 0x" + shot.hashTable.ToString("X"));
            sw.Flush();
#endif
            FileStream fs = new FileStream(shot.fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] file = new byte[shot.jpegLength];
            int count = 0;
            int copyTo = 0;
            int thisBlock = shot.headerStart;
            int headerLength = shot.headerLength;
            int numBlocks = shot.numBlocks;
            short blockRemain = (short)((numBlocks * 4096) - shot.jpegLength - headerLength);
            if (blockRemain > 4096)
            {
#if DEBUG
                sw.WriteLine("// Block Remainder: ((" + shot.numBlocks + " * 4096) - " + shot.jpegLength + " - " + shot.headerLength + ") = " + blockRemain + " //");
                sw.WriteLine("// Block remainder is greater than 4096, ignoring last block //");
#endif
                numBlocks--;
            }
            while (headerLength >= 4096) // This should only happen in Reach Retail files, because of the extra length of the SCNC block
            {
                numBlocks--;
                headerLength -= 4096;
                br.BaseStream.Position = shot.hashTable + ((thisBlock * 24) + 21);
                thisBlock = br.ReadBigInt24();
            }
#if DEBUG
            sw.WriteLine();
            sw.Flush();
#endif
            while (count < numBlocks)
            {
                br.BaseStream.Position = (thisBlock * 4096) + 49152;
                byte[] buffer;
                int readBytes = 4096;

                if (count == 0)
                {
                    br.BaseStream.Position += headerLength;
                    readBytes -= headerLength;
                }
                else if (count == (numBlocks - 1))
                {
                    readBytes -= (numBlocks * 4096) - headerLength - shot.jpegLength;
                }
#if DEBUG
                sw.WriteLine("count: " + count.ToString().PadLeft(shot.numBlocks.ToString().Length, ' ') +
                         ", thisBlock: " + thisBlock.ToString().PadLeft(shot.numBlocks.ToString().Length, ' ') +
                         ", hashTable: " + (int)(shot.hashTable + ((thisBlock * 24) + 21)) +
                         ", reading " + readBytes.ToString().PadLeft(4, ' ') + " bytes from " +
                         br.BaseStream.Position.ToString().PadLeft(shot.jpegLength.ToString().Length, ' ') +
                         " and copying to " + copyTo.ToString().PadLeft(shot.jpegLength.ToString().Length, ' '));
                sw.Flush();
#endif
                buffer = br.ReadBytes(readBytes);
                br.BaseStream.Position = shot.hashTable + ((thisBlock * 24) + 21);
                thisBlock = br.ReadBigInt24();
                count++;

                try
                {
                    buffer.CopyTo(file, copyTo);
                }
                catch (Exception e)
                {
#if DEBUG
                    sw.WriteLine();
                    sw.WriteLine("// Error occured at count " + count + ": " + e.Message + "//");
                    sw.WriteLine();
                    sw.Flush();
#endif
                }

                copyTo += readBytes;
            }

            br.Close();
            fs.Close();
#if DEBUG
            sw.Close();
            debug.Close();
#endif
            return file;
        }

        internal screenshotItem shotHalo3(BinaryReader br, int fileStart, screenshotItem screenshot)
        {
            br.BaseStream.Position = fileStart + 4;
            int blfLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 4;
            int chdrLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 12;
            int blfVersion = br.ReadBigInt16();
            br.BaseStream.Position += 10; // Move forward 10 bytes to get to the start of the title
            string shotTitle = Encoding.BigEndianUnicode.GetString(br.ReadBytes(32)).Trim('\0');
            br.BaseStream.Position += 172; // Move forward 172 bytes to get to the shot time
            int shotTime = br.ReadBigInt32();
            br.BaseStream.Position += 8; // Move forward 8 bytes to the Map ID
            int mapId = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + chdrLength + 4;
            int scncLength = 0; // The scnc block doesn't exist in versions prior to 12070, so we'll check for that
            if(blfVersion >= 12070)
                scncLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + chdrLength + scncLength + 12;
            int jpegLength = br.ReadBigInt32();
            int jpegOffset = (int)br.BaseStream.Position;

            screenshot.blfVersion = blfVersion;
            screenshot.headerLength = blfLength + chdrLength + scncLength + 16;
            screenshot.headerStart = (fileStart - 49152) / 4096;
            screenshot.jpegLength = jpegLength;
            screenshot.jpegOffset = jpegOffset;
            screenshot.fileTime = shotTime;
            screenshot.mapID = mapId;
            screenshot.shotTitle = shotTitle;
            
            return screenshot;
        }

        internal screenshotItem shotHalo3ODST(BinaryReader br, int fileStart, screenshotItem screenshot)
        {
            br.BaseStream.Position = fileStart + 4;
            int blfLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 4;
            int chdrLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 12;
            int blfVersion = br.ReadBigInt16();
            br.BaseStream.Position += 10; // Move forward 10 bytes to get to the start of the title
            string shotTitle = Encoding.BigEndianUnicode.GetString(br.ReadBytes(32)).Trim('\0');
            br.BaseStream.Position += 172; // Move forward 172 bytes to get to the shot time
            int shotTime = br.ReadBigInt32();
            br.BaseStream.Position += 8;
            int mapId = br.ReadBigInt32();
            br.BaseStream.Position += 8;
            byte fireFight = br.ReadByte();
            if (fireFight != 0)
                mapId += 10000 + fireFight;
            br.BaseStream.Position = fileStart + blfLength + chdrLength + 4;
            int scncLength = 0; // The scnc block doesn't exist in versions prior to 12070, so we'll check for that
            if (blfVersion >= 12070)
                scncLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + chdrLength + scncLength + 12;
            int jpegLength = br.ReadBigInt32();
            int jpegOffset = (int)br.BaseStream.Position;

            screenshot.blfVersion = blfVersion;
            screenshot.headerLength = blfLength + chdrLength + scncLength + 16;
            screenshot.headerStart = (fileStart - 49152) / 4096;
            screenshot.jpegLength = jpegLength;
            screenshot.jpegOffset = jpegOffset;
            screenshot.fileTime = shotTime;
            screenshot.mapID = mapId;
            screenshot.shotTitle = shotTitle;

            return screenshot;
        }

        internal screenshotItem shotReachBeta(BinaryReader br, int fileStart, screenshotItem screenshot)
        {
            br.BaseStream.Position = fileStart + 4;
            int blfLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 4;
            int chdrLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 12;
            int blfVersion = br.ReadBigInt16();
            br.BaseStream.Position += 46; // Move forward 46 bytes to get to the mapID
            int mapId = br.ReadBigInt32() + 10000;
            br.BaseStream.Position += 12; // Move forward 12 to get to the created time
            int shotTime = br.ReadBigInt32();
            br.BaseStream.Position += 64; // Move forward 64 bytes to get to the title
            string shotTitle = Encoding.BigEndianUnicode.GetString(br.ReadBytes(256)).Trim('\0');
            br.BaseStream.Position = fileStart + blfLength + chdrLength + 4;
            int scncLength = 0; // The scnc block doesn't exist in Halo: Reach Beta screenshots
            br.BaseStream.Position = fileStart + blfLength + chdrLength + scncLength + 12;
            int jpegLength = br.ReadBigInt32();
            int jpegOffset = (int)br.BaseStream.Position;

            screenshot.blfVersion = blfVersion;
            screenshot.headerLength = blfLength + chdrLength + scncLength + 16;
            screenshot.headerStart = (fileStart - 49152) / 4096;
            screenshot.jpegLength = jpegLength;
            screenshot.jpegOffset = jpegOffset;
            screenshot.mapID = mapId;
            screenshot.fileTime = shotTime;
            screenshot.shotTitle = shotTitle;

            return screenshot;
        }

        internal screenshotItem shotReach(BinaryReader br, int fileStart, screenshotItem screenshot)
        {
            int headerStart = (fileStart - 49152) / 4096;
            br.BaseStream.Position = fileStart + 4;
            int blfLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 4;
            int chdrLength = br.ReadBigInt32();
            br.BaseStream.Position = fileStart + blfLength + 12;
            int blfVersion = br.ReadBigInt16();
            br.BaseStream.Position += 46; // Move forward 46 bytes to get to the mapID
            int mapId = br.ReadBigInt32();
            if (screenshot.gameID == 4 && mapId == 10080)
                mapId += 1; // This is done to prevent a discrepancy with Halo: Reach's "Installation 04"
            br.BaseStream.Position += 12; // Move forward 12 bytes to get to the created timestamp
            int createTime = br.ReadBigInt32();
            br.BaseStream.Position += 32;
            int modTime = br.ReadBigInt32();
            br.BaseStream.Position += 28; // Move forward 28 bytes to get to the title
            string shotTitle = Encoding.BigEndianUnicode.GetString(br.ReadBytes(256)).Trim('\0');
            br.BaseStream.Position = fileStart + blfLength + chdrLength + 4;
            int scncLength = br.ReadBigInt32();
            br.BaseStream.Position = screenshot.hashTable + ((headerStart * 24) + 21); // Since the header in Reach Retail screenshot files spans more than one block, we need to find out where to go next from the hash table
            int nextBlock = (br.ReadBigInt24() * 4096) + 49152;
            int bytesRemain = (blfLength + chdrLength + scncLength) - 4096;
            br.BaseStream.Position = nextBlock + bytesRemain + 12;
            int jpegLength = br.ReadBigInt32();
            int jpegOffset = (int)br.BaseStream.Position;

            screenshot.blfVersion = blfVersion;
            screenshot.headerLength = blfLength + chdrLength + scncLength + 16;
            screenshot.headerStart = headerStart;
            screenshot.jpegLength = jpegLength;
            screenshot.jpegOffset = jpegOffset;
            screenshot.mapID = mapId;
            screenshot.fileTime = createTime;
            screenshot.shotTitle = shotTitle;

            return screenshot;
        }
    }
}