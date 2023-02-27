﻿//============================================================================
// BDInfo - Blu-ray Video and Audio Analysis Tool
// Copyright © 2010 Cinema Squid
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

namespace BDInfoLib.BDROM;

public class TSStreamClip
{
    public int AngleIndex = 0;
    public string Name;
    public double TimeIn;
    public double TimeOut;
    public double RelativeTimeIn;
    public double RelativeTimeOut;
    public double Length;
    public double RelativeLength;

    public ulong FileSize;
    public ulong InterleavedFileSize;
    public ulong PayloadBytes = 0;
    public ulong PacketCount = 0;
    public double PacketSeconds = 0;

    public List<double> Chapters = new();

    public TSStreamFile StreamFile;
    public TSStreamClipFile StreamClipFile;

    public TSStreamClip(TSStreamFile streamFile, TSStreamClipFile streamClipFile)
    {
        if (streamFile != null)
        {
            Name = streamFile.Name;
            StreamFile = streamFile;

            if (StreamFile.FileInfo != null)
                FileSize = (ulong)StreamFile.FileInfo.Length;

            if (StreamFile.InterleavedFile is { FileInfo: { } }) 
                InterleavedFileSize = (ulong)StreamFile.InterleavedFile.FileInfo.Length;
        }
        StreamClipFile = streamClipFile;
    }

    public string DisplayName
    {
        get
        {
            if (StreamFile is { InterleavedFile: { } } && BDInfoLibSettings.EnableSSIF)
            {
                return StreamFile.InterleavedFile.Name;
            }
            return Name;
        }
    }

    public ulong PacketSize => PacketCount * 192;

    public ulong PacketBitRate
    {
        get
        {
            if (PacketSeconds > 0)
            {
                return (ulong)Math.Round(PacketSize * 8.0 / PacketSeconds);
            }
            return 0;
        }
    }

    public bool IsCompatible(TSStreamClip clip)
    {
        foreach (var stream1 in StreamFile.Streams.Values)
        {
            if (!clip.StreamFile.Streams.ContainsKey(stream1.PID)) continue;

            var stream2 = clip.StreamFile.Streams[stream1.PID];
            if (stream1.StreamType != stream2.StreamType)
            {
                return false;
            }
        }
        return true;
    }
}