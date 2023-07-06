﻿/**
* Digital Voice Modem - Fixed Network Equipment
* AGPLv3 Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
* @package DVM / Fixed Network Equipment
*
*/
//
// Based on code from the MMDVMHost project. (https://github.com/g4klx/MMDVMHost)
// Licensed under the GPLv2 License (https://opensource.org/licenses/GPL-2.0)
//
/*
*   Copyright (C) 2022 by Bryan Biedenkapp N2PLL
*
*   This program is free software: you can redistribute it and/or modify
*   it under the terms of the GNU Affero General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU Affero General Public License for more details.
*/

using System;
using System.Runtime.InteropServices;

namespace dvmbridge.FNE.EDAC
{
    /// <summary>
    /// Implements various Cyclic Redundancy Check routines.
    /// </summary>
    public sealed class CRC
    {
        public static readonly byte[] CRC8_TABLE = new byte[257] {
            0x00, 0x07, 0x0E, 0x09, 0x1C, 0x1B, 0x12, 0x15, 0x38, 0x3F, 0x36, 0x31,
            0x24, 0x23, 0x2A, 0x2D, 0x70, 0x77, 0x7E, 0x79, 0x6C, 0x6B, 0x62, 0x65,
            0x48, 0x4F, 0x46, 0x41, 0x54, 0x53, 0x5A, 0x5D, 0xE0, 0xE7, 0xEE, 0xE9,
            0xFC, 0xFB, 0xF2, 0xF5, 0xD8, 0xDF, 0xD6, 0xD1, 0xC4, 0xC3, 0xCA, 0xCD,
            0x90, 0x97, 0x9E, 0x99, 0x8C, 0x8B, 0x82, 0x85, 0xA8, 0xAF, 0xA6, 0xA1,
            0xB4, 0xB3, 0xBA, 0xBD, 0xC7, 0xC0, 0xC9, 0xCE, 0xDB, 0xDC, 0xD5, 0xD2,
            0xFF, 0xF8, 0xF1, 0xF6, 0xE3, 0xE4, 0xED, 0xEA, 0xB7, 0xB0, 0xB9, 0xBE,
            0xAB, 0xAC, 0xA5, 0xA2, 0x8F, 0x88, 0x81, 0x86, 0x93, 0x94, 0x9D, 0x9A,
            0x27, 0x20, 0x29, 0x2E, 0x3B, 0x3C, 0x35, 0x32, 0x1F, 0x18, 0x11, 0x16,
            0x03, 0x04, 0x0D, 0x0A, 0x57, 0x50, 0x59, 0x5E, 0x4B, 0x4C, 0x45, 0x42,
            0x6F, 0x68, 0x61, 0x66, 0x73, 0x74, 0x7D, 0x7A, 0x89, 0x8E, 0x87, 0x80,
            0x95, 0x92, 0x9B, 0x9C, 0xB1, 0xB6, 0xBF, 0xB8, 0xAD, 0xAA, 0xA3, 0xA4,
            0xF9, 0xFE, 0xF7, 0xF0, 0xE5, 0xE2, 0xEB, 0xEC, 0xC1, 0xC6, 0xCF, 0xC8,
            0xDD, 0xDA, 0xD3, 0xD4, 0x69, 0x6E, 0x67, 0x60, 0x75, 0x72, 0x7B, 0x7C,
            0x51, 0x56, 0x5F, 0x58, 0x4D, 0x4A, 0x43, 0x44, 0x19, 0x1E, 0x17, 0x10,
            0x05, 0x02, 0x0B, 0x0C, 0x21, 0x26, 0x2F, 0x28, 0x3D, 0x3A, 0x33, 0x34,
            0x4E, 0x49, 0x40, 0x47, 0x52, 0x55, 0x5C, 0x5B, 0x76, 0x71, 0x78, 0x7F,
            0x6A, 0x6D, 0x64, 0x63, 0x3E, 0x39, 0x30, 0x37, 0x22, 0x25, 0x2C, 0x2B,
            0x06, 0x01, 0x08, 0x0F, 0x1A, 0x1D, 0x14, 0x13, 0xAE, 0xA9, 0xA0, 0xA7,
            0xB2, 0xB5, 0xBC, 0xBB, 0x96, 0x91, 0x98, 0x9F, 0x8A, 0x8D, 0x84, 0x83,
            0xDE, 0xD9, 0xD0, 0xD7, 0xC2, 0xC5, 0xCC, 0xCB, 0xE6, 0xE1, 0xE8, 0xEF,
            0xFA, 0xFD, 0xF4, 0xF3, 0x01 };

        public static readonly ushort[] CRC9_TABLE = new ushort[135] {
            0x1E7, 0x1F3, 0x1F9, 0x1FC, 0x0D2, 0x045, 0x122, 0x0BD, 0x15E, 0x083,
            0x141, 0x1A0, 0x0FC, 0x052, 0x005, 0x102, 0x0AD, 0x156, 0x087, 0x143,
            0x1A1, 0x1D0, 0x0C4, 0x04E, 0x00B, 0x105, 0x182, 0x0ED, 0x176, 0x097,
            0x14B, 0x1A5, 0x1D2, 0x0C5, 0x162, 0x09D, 0x14E, 0x08B, 0x145, 0x1A2,
            0x0FD, 0x17E, 0x093, 0x149, 0x1A4, 0x0FE, 0x053, 0x129, 0x194, 0x0E6,
            0x05F, 0x12F, 0x197, 0x1CB, 0x1E5, 0x1F2, 0x0D5, 0x16A, 0x099, 0x14C,
            0x08A, 0x069, 0x134, 0x0B6, 0x077, 0x13B, 0x19D, 0x1CE, 0x0CB, 0x165,
            0x1B2, 0x0F5, 0x17A, 0x091, 0x148, 0x088, 0x068, 0x018, 0x020, 0x03C,
            0x032, 0x035, 0x11A, 0x0A1, 0x150, 0x084, 0x06E, 0x01B, 0x10D, 0x186,
            0x0EF, 0x177, 0x1BB, 0x1DD, 0x1EE, 0x0DB, 0x16D, 0x1B6, 0x0F7, 0x17B,
            0x1BD, 0x1DE, 0x0C3, 0x161, 0x1B0, 0x0F4, 0x056, 0x007, 0x103, 0x181,
            0x1C0, 0x0CC, 0x04A, 0x009, 0x104, 0x0AE, 0x07B, 0x13D, 0x19E, 0x0E3,
            0x171, 0x1B8, 0x0F0, 0x054, 0x006, 0x02F, 0x117, 0x18B, 0x1C5, 0x1E2,
            0x0DD, 0x16E, 0x09B, 0x14D, 0x1A6 };

        public static readonly ushort[] CCITT16_TABLE1 = new ushort[256] {
            0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
            0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
            0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
            0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
            0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
            0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
            0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
            0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
            0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
            0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
            0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
            0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
            0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
            0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
            0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
            0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
            0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
            0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
            0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
            0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
            0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
            0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
            0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
            0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
            0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
            0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
            0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
            0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
            0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
            0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
            0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
            0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78 };

        public static readonly ushort[] CCITT16_TABLE2 = new ushort[256] {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
            0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
            0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
            0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
            0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
            0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
            0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
            0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
            0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
            0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
            0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
            0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
            0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
            0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
            0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
            0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
            0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
            0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
            0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
            0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
            0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
            0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
            0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0 };

        public static readonly uint[] CRC32_TABLE = new uint[256] {
            0x00000000, 0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, 0x17C56B6B, 0x1A864DB2, 0x1E475005,
            0x2608EDB8, 0x22C9F00F, 0x2F8AD6D6, 0x2B4BCB61, 0x350C9B64, 0x31CD86D3, 0x3C8EA00A, 0x384FBDBD,
            0x4C11DB70, 0x48D0C6C7, 0x4593E01E, 0x4152FDA9, 0x5F15ADAC, 0x5BD4B01B, 0x569796C2, 0x52568B75,
            0x6A1936C8, 0x6ED82B7F, 0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3, 0x709F7B7A, 0x745E66CD,
            0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, 0x95609039, 0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5,
            0xBE2B5B58, 0xBAEA46EF, 0xB7A96036, 0xB3687D81, 0xAD2F2D84, 0xA9EE3033, 0xA4AD16EA, 0xA06C0B5D,
            0xD4326D90, 0xD0F37027, 0xDDB056FE, 0xD9714B49, 0xC7361B4C, 0xC3F706FB, 0xCEB42022, 0xCA753D95,
            0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1, 0xE13EF6F4, 0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D,
            0x34867077, 0x30476DC0, 0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C, 0x2E003DC5, 0x2AC12072,
            0x128E9DCF, 0x164F8078, 0x1B0CA6A1, 0x1FCDBB16, 0x018AEB13, 0x054BF6A4, 0x0808D07D, 0x0CC9CDCA,
            0x7897AB07, 0x7C56B6B0, 0x71159069, 0x75D48DDE, 0x6B93DDDB, 0x6F52C06C, 0x6211E6B5, 0x66D0FB02,
            0x5E9F46BF, 0x5A5E5B08, 0x571D7DD1, 0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D, 0x40D816BA,
            0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E, 0xBFA1B04B, 0xBB60ADFC, 0xB6238B25, 0xB2E29692,
            0x8AAD2B2F, 0x8E6C3698, 0x832F1041, 0x87EE0DF6, 0x99A95DF3, 0x9D684044, 0x902B669D, 0x94EA7B2A,
            0xE0B41DE7, 0xE4750050, 0xE9362689, 0xEDF73B3E, 0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2,
            0xC6BCF05F, 0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683, 0xD1799B34, 0xDC3ABDED, 0xD8FBA05A,
            0x690CE0EE, 0x6DCDFD59, 0x608EDB80, 0x644FC637, 0x7A089632, 0x7EC98B85, 0x738AAD5C, 0x774BB0EB,
            0x4F040D56, 0x4BC510E1, 0x46863638, 0x42472B8F, 0x5C007B8A, 0x58C1663D, 0x558240E4, 0x51435D53,
            0x251D3B9E, 0x21DC2629, 0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5, 0x3F9B762C, 0x3B5A6B9B,
            0x0315D626, 0x07D4CB91, 0x0A97ED48, 0x0E56F0FF, 0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623,
            0xF12F560E, 0xF5EE4BB9, 0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2, 0xE6EA3D65, 0xEBA91BBC, 0xEF68060B,
            0xD727BBB6, 0xD3E6A601, 0xDEA580D8, 0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD, 0xCDA1F604, 0xC960EBB3,
            0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7, 0xAE3AFBA2, 0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B,
            0x9B3660C6, 0x9FF77D71, 0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD, 0x81B02D74, 0x857130C3,
            0x5D8A9099, 0x594B8D2E, 0x5408ABF7, 0x50C9B640, 0x4E8EE645, 0x4A4FFBF2, 0x470CDD2B, 0x43CDC09C,
            0x7B827D21, 0x7F436096, 0x7200464F, 0x76C15BF8, 0x68860BFD, 0x6C47164A, 0x61043093, 0x65C52D24,
            0x119B4BE9, 0x155A565E, 0x18197087, 0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B, 0x0FDC1BEC,
            0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088, 0x2497D08D, 0x2056CD3A, 0x2D15EBE3, 0x29D4F654,
            0xC5A92679, 0xC1683BCE, 0xCC2B1D17, 0xC8EA00A0, 0xD6AD50A5, 0xD26C4D12, 0xDF2F6BCB, 0xDBEE767C,
            0xE3A1CBC1, 0xE760D676, 0xEA23F0AF, 0xEEE2ED18, 0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4,
            0x89B8FD09, 0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5, 0x9E7D9662, 0x933EB0BB, 0x97FFAD0C,
            0xAFB010B1, 0xAB710D06, 0xA6322BDF, 0xA2F33668, 0xBCB4666D, 0xB8757BDA, 0xB5365D03, 0xB1F740B4 };

        /*
        ** Structures
        **
        ** bryanb: Please don't modify these following structures, they are specially designed to abuse
        ** how the .NET CLR handles memory management, such that these effectively emulate C/C++ unions.
        */
#pragma warning disable CS0649
        /// <summary>
        /// 
        /// </summary>
        private struct DoubleByte
        {
            /// <summary>
            /// 
            /// </summary>
            public byte B0;
            /// <summary>
            /// 
            /// </summary>
            public byte B1;
        } // private struct DoubleByte

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 2)]
        private struct UShortUnion
        {
            [FieldOffset(0)]
            public ushort crc16;
            [FieldOffset(0)]
            public DoubleByte crc8;
        } // private struct UShortUnion

        /// <summary>
        /// 
        /// </summary>
        private struct QuadByte
        {
            /// <summary>
            /// 
            /// </summary>
            public byte B0;
            /// <summary>
            /// 
            /// </summary>
            public byte B1;
            /// <summary>
            /// 
            /// </summary>
            public byte B2;
            /// <summary>
            /// 
            /// </summary>
            public byte B3;
        } // private struct QuadByte

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        private struct UIntUnion
        {
            [FieldOffset(0)]
            public uint crc32;
            [FieldOffset(0)]
            public QuadByte crc8;
        } // private struct UIntUnion
#pragma warning restore CS0649
        /*
        ** Methods
        */

        /// <summary>
        /// Check 5-bit CRC.
        /// </summary>
        /// <param name="in">Boolean bit array.</param>
        /// <param name="tcrc">Computed CRC to check.</param>
        /// <returns>True, if CRC is valid, otherwise false.</returns>
        public static bool CheckFiveBit(bool[] _in, uint tcrc)
        {
            if (_in == null)
                throw new NullReferenceException("_in");

            uint crc = 0x0;
            EncodeFiveBit(_in, ref crc);

            return crc == tcrc;
        }

        /// <summary>
        /// Encode 5-bit CRC.
        /// </summary>
        /// <param name="in">Boolean bit array.</param>
        /// <param name="tcrc">Computed CRC.</param>
        public static void EncodeFiveBit(bool[] _in, ref uint tcrc)
        {
            if (_in == null)
                throw new NullReferenceException("_in");

            ushort total = 0;
            for (uint i = 0U; i < 72U; i += 8U) {
                byte c = 0x0;
                FneUtils.BitsToByteBE(_in, (int)i, ref c);
                total += c;
            }

            total %= (byte)(31U);

            tcrc = total;
        }

        /// <summary>
        /// Check 16-bit CRC-CCITT.
        /// </summary>
        /// <remarks>This uses polynomial 0x1021.</remarks>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        /// <returns>True, if CRC is valid, otherwise false.</returns>
        public static bool CheckCCITT162(byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 2)
                throw new ArgumentOutOfRangeException("length");

            UShortUnion union = new UShortUnion();
            union.crc16 = 0;

            for (uint i = 0U; i < (length - 2U); i++)
                union.crc16 = (ushort)((union.crc8.B0 << 8) ^ CCITT16_TABLE2[union.crc8.B1 ^ _in[i]]);

            union.crc16 = (ushort)(~union.crc16);

            return union.crc8.B0 == _in[length - 1U] && union.crc8.B1 == _in[length - 2U];
        }

        /// <summary>
        /// Encode 16-bit CRC-CCITT.
        /// </summary>
        /// <remarks>This uses polynomial 0x1021.</remarks>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        public static void AddCCITT162(ref byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 2)
                throw new ArgumentOutOfRangeException("length");

            UShortUnion union = new UShortUnion();
            union.crc16 = 0;

            for (uint i = 0U; i < (length - 2U); i++)
                union.crc16 = (ushort)((union.crc8.B0 << 8) ^ CCITT16_TABLE2[union.crc8.B1 ^ _in[i]]);

            union.crc16 = (ushort)(~union.crc16);

            _in[length - 1U] = union.crc8.B0;
            _in[length - 2U] = union.crc8.B1;
        }

        /// <summary>
        /// Check 16-bit CRC-CCITT.
        /// </summary>
        /// <remarks>This uses polynomial 0x1189.</remarks>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        /// <returns>True, if CRC is valid, otherwise false.</returns>
        public static bool CheckCCITT161(byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 2)
                throw new ArgumentOutOfRangeException("length");

            UShortUnion union = new UShortUnion();
            union.crc16 = 0xFFFF;

            for (uint i = 0U; i < (length - 2U); i++)
                union.crc16 = (ushort)(union.crc8.B1 ^ CCITT16_TABLE1[union.crc8.B0 ^ _in[i]]);

            union.crc16 = (ushort)(~union.crc16);

            return union.crc8.B0 == _in[length - 1U] && union.crc8.B1 == _in[length - 2U];
        }

        /// <summary>
        /// Encode 16-bit CRC-CCITT.
        /// </summary>
        /// <remarks>This uses polynomial 0x1189.</remarks>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        public static void AddCCITT161(ref byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 2)
                throw new ArgumentOutOfRangeException("length");

            UShortUnion union = new UShortUnion();
            union.crc16 = 0xFFFF;

            for (uint i = 0U; i < (length - 2U); i++)
                union.crc16 = (ushort)(union.crc8.B1 ^ CCITT16_TABLE1[union.crc8.B0 ^ _in[i]]);

            union.crc16 = (ushort)(~union.crc16);

            _in[length - 2U] = union.crc8.B0;
            _in[length - 1U] = union.crc8.B1;
        }

        /// <summary>
        /// Check 32-bit CRC.
        /// </summary>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        /// <returns>True, if CRC is valid, otherwise false.</returns>
        public static bool CheckCRC32(byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 4)
                throw new ArgumentOutOfRangeException("length");

            UIntUnion union = new UIntUnion();
            union.crc32 = 0;

            uint i = 0;
            for (uint j = (length - 4U); j-- > 0; i++) {
                uint idx = ((union.crc32 >> 24) ^ _in[i]) & 0xFFU;
                union.crc32 = (CRC32_TABLE[idx] ^ (union.crc32 << 8)) & 0xFFFFFFFFU;
            }

            union.crc32 = ~union.crc32;
            union.crc32 &= 0xFFFFFFFFU;

            return union.crc8.B0 == _in[length - 1U] && union.crc8.B1 == _in[length - 2U] && union.crc8.B2 == _in[length - 3U] && union.crc8.B3 == _in[length - 4U];
        }

        /// <summary>
        /// Encode 32-bit CRC.
        /// </summary>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        public static void AddCRC32(ref byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");
            if (length > 4)
                throw new ArgumentOutOfRangeException("length");

            UIntUnion union = new UIntUnion();
            union.crc32 = 0;

            uint i = 0;
            for (uint j = (length - 4U); j-- > 0; i++)
            {
                uint idx = ((union.crc32 >> 24) ^ _in[i]) & 0xFFU;
                union.crc32 = (CRC32_TABLE[idx] ^ (union.crc32 << 8)) & 0xFFFFFFFFU;
            }

            union.crc32 = ~union.crc32;
            union.crc32 &= 0xFFFFFFFFU;

            _in[length - 1U] = union.crc8.B0;
            _in[length - 2U] = union.crc8.B1;
            _in[length - 3U] = union.crc8.B2;
            _in[length - 4U] = union.crc8.B3;
        }

        /// <summary>
        /// Generate 8-bit CRC.
        /// </summary>
        /// <param name="in">Input byte array.</param>
        /// <param name="length">Length of byte array.</param>
        /// <returns>Calculated 8-bit CRC value.</returns>
        public static byte Crc8(byte[] _in, uint length)
        {
            if (_in == null)
                throw new NullReferenceException("_in");

            byte crc = 0;

            for (uint i = 0U; i < length; i++)
                crc = CRC8_TABLE[crc ^ _in[i]];

            return crc;
        }

        /// <summary>
        /// Generate 9-bit CRC.
        /// </summary>
        /// <param name="in">Input byte array.</param>
        /// <param name="bitLength">Length of byte array in bits.</param>
        /// <returns>Calculated 9-bit CRC value.</returns>
        public static ushort Crc9(byte[] _in, uint bitLength)
        {
            if (_in == null)
                throw new NullReferenceException("_in");

            ushort crc = 0;

            for (uint i = 0; i < bitLength; i++) {
                bool b = FneUtils.ReadBit(_in, i);
                if (b) {
                    if (i < 7U) {
                        crc ^= CRC9_TABLE[i];
                    } else if (i > 15) {
                        crc ^= CRC9_TABLE[i - 9];
                    }
                }
            }

            // crc = ~crc;
            crc &= (ushort)0x1FFU;
            crc ^= (ushort)0x1FFU;
    
            return crc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="in">Input byte array.</param>
        /// <param name="bitLength">Length of byte array in bits.</param>
        /// <returns></returns>
        public static ushort CreateCRC16(byte[] _in, uint bitLength)
        {
            ushort crc = (ushort)0xFFFFU;

            for (uint i = 0U; i < bitLength; i++) {
                bool bit1 = FneUtils.ReadBit(_in, i);
                bool bit2 = (crc & 0x8000U) == 0x8000U;

                crc <<= 1;

                if (bit1 ^ bit2)
                    crc ^= (ushort)0x1021U;
            }

            return (ushort)(crc & 0xFFFFU);
        }
    } // public sealed class CRC
} // namespace dvmbridge.FNE.EDAC