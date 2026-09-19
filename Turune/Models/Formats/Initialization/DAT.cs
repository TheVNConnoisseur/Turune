using System;
using System.Collections.Generic;
using System.Text;

namespace Turune.Models.Formats.Initialization
{
    internal class DAT
    {
        internal class DATScheme
        {
            //When parsing the DAT file, the game does it in 3 parts, the same is done here, since each
            //part has a different size and structure.
            public DATScheme_Part1 Part1 { get; set; } = new DATScheme_Part1();
            public DATScheme_Part2 Part2 { get; set; } = new DATScheme_Part2();
            public DATScheme_Part3 Part3 { get; set; } = new DATScheme_Part3();
        }
        //0x2194 bytes (hardcoded value)
        internal class DATScheme_Part1
        {
            public DateTime GameIdentifier { get; set; } = DateTime.MinValue;
            public uint GameSystemMode { get; set; }
            public byte ExclusiveFullscreen { get; set; }
            public byte LocaleEnglish { get; set; }
            public string WindowTitle { get; set; } = "";
            public byte DATKey { get; set; }
            public ushort WindowWidth { get; set; }
            public ushort WindowHeight { get; set; }
            public byte[] Reserved0 { get; set; } = [0xD0, 0x02];

            //Flags
            public int DI_Flags { get; set; }
            public int DG_Flags { get; set; }
            public int DT_Flags { get; set; }
            public int SI_Flags { get; set; }
            public int SG_Flags { get; set; }
            public int ST_Flags { get; set; }

            //Slots
            public int Obj_Slots { get; set; }
            public int MSK_Slots { get; set; }
            public int CGB_Slots { get; set; }
            public int Box_Slots { get; set; }
            public int MessageLog_Slots { get; set; }

            //obj IDs
            public int ObjID_MessageWindow { get; set; }
            public int ObjID_MaxValueOnBoot { get; set; }

            //Folder names
            public string SDT_FolderName { get; set; } = "data06";
            public string MG2_FolderName { get; set; } = "data00";
            public string MAL_FolderName { get; set; } = "data02";
            public string BGM_FolderName { get; set; } = "data05";
            public string Voice_FolderName { get; set; } = "data03";
            public string SE_FolderName { get; set; } = "data04";
            public string AM2_FolderName { get; set; } = "data01";
            public string Unused_FolderName { get; set; } = "";

            //Save files
            public string SaveData_FolderName { get; set; } = "data";
            public string SaveData_FileName { get; set; } = "Flag.odn";

            //Close game process
            public byte CloseGame_UseMessageBox { get; set; }
            public byte Reserved1 { get; set; } = 0;
            public ushort CloseGame_SeenNumber { get; set; }
            public string CloseGame_JumpLabel { get; set; } = "";

            //Screen mode process
            public ushort ScreenMode_ActivateFullScreen_SeenNumber { get; set; }
            public string ScreenMode_ActivateFullScreen_JumpLabel { get; set; } = "";
            public ushort ScreenMode_ActivateWindowed_SeenNumber { get; set; }
            public string ScreenMode_ActivateWindowed_JumpLabel { get; set; } = "";

            //Fonts
            public string FontName { get; set; } = "ＭＳ ゴシック";
            public DATScheme_Font[] Font { get; set; } = DefaultFonts();  //50 fonts in total

            //Message window: color customization
            public string[] MessageWindow_CustomColorCharName_CharName { get; set; } = new string[50];
            public byte[][] MessageWindow_CustomColorCharName_RGB { get; set; } = new byte[50][];
            public byte[] MessageWindow_CharacterName_DefaultRGB { get; set; } = [0xFF, 0xFF, 0xFF];
            public byte MessageWindow_CustomColorCharName_Enabled { get; set; } = 1;
            public byte[] MessageWindow_Message_DefaultRGB { get; set; } = [0xFF, 0xFF, 0xFF];

            //Background fill
            public byte[] BackgroundFill_RGB { get; set; } = [0x00, 0x00, 0x00];
            public byte BackgroundFill_Opacity { get; set; } = 0xC8;

            //Reserved: some of these values get copied directly to the save game file, but they are not used by the game in any way.
            public byte ReservedSaveGame0 { get; set; } = 1;
            public byte ReservedSaveGame1 { get; set; } = 1;
            public byte[] Reserved2 { get; set; } = [0xA, 0x00];

            //Message window: font used and order of the RECTS
            public ushort MessageWindow_FontID { get; set; } = 0;
            public ushort MessageWindow_RECTOrder_SurfaceImage { get; set; } = 0;
            public ushort MessageWindow_RECTOrder_Image { get; set; } = 1;
            public ushort MessageWindow_RECTOrder_SurfaceMessageText { get; set; } = 2;
            public ushort MessageWindow_RECTOrder_SurfaceCharacterNameText { get; set; } = 3;
            public ushort MessageWindow_RECTOrder_MessageText { get; set; } = 4;
            public ushort MessageWindow_RECTOrder_CharacterNameText { get; set; } = 5;

            //Choice boxes: obj IDs slots
            public ushort ChoiceBox_FirstID { get; set; } = 75;
            public ushort ChoiceBox_LastID { get; set; } = 99;

            //Audio instances
            public ushort AudioInstances_TotalAmount { get; set; } = 25;
            public ushort AudioInstances_BGM { get; set; } = 0;
            public ushort AudioInstances_Voice { get; set; } = 1;
            public byte AudioInstances_Unused { get; set; } = 50;

            //SDT key
            public byte SDTKey_Shuffler { get; set; } = 0;
            public byte[] SDTKey { get; set; } = new byte[260];

            //Methods
            private static DATScheme_Font[] DefaultFonts()
            {
                var DefaultFonts = new DATScheme_Font[50];
                for (int i = 0; i < 50; i++) DefaultFonts[i] = new DATScheme_Font();

                //Setting up the fonts the game actually uses and modifies its values
                DefaultFonts[0].Height = 24;
                DefaultFonts[0].DropShadow_RGBA = [0x64, 0x00, 0x00, 0x00];
                DefaultFonts[1].Height = 31;
                DefaultFonts[1].DropShadow_RGBA = [0x64, 0x00, 0x00, 0x00];
                return DefaultFonts;
            }
        }

        //0x37F0 bytes (hardcoded value)
        internal class DATScheme_Part2
        {

        }

        //0x6A90 bytes (hardcoded value)
        internal class DATScheme_Part3
        {
        }

        internal class DATScheme_Font
        {
            public int Height { get; set; } = 25;
            public int Width { get; set; } = 0;
            public int Weight { get; set; } = 44;
            public int DropShadow_HorizontalOffset { get; set; } = 1;
            public int DropShadow_VerticalOffset { get; set; } = 1;
            public byte[] DropShadow_RGBA { get; set; } = [0x41, 0x00, 0x00, 0x00]; //RGBA is a class present only in .NET 11 and above (https://learn.microsoft.com/en-us/dotnet/api/system.numerics.colors.rgba?view=net-11.0)
            public uint Reserved { get; set; }
        }
    }
}

