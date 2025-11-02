using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public sealed unsafe class VideoReader : IDisposable
    {
        public readonly AVFormatContext* FormatContext;
        private readonly AVPacket* _pPacket;
         
        public int StreamIndex;
        public readonly AVCodec* Codec;

        public VideoReader(string url)
        {
            AVDictionary* dictionary = null;
            ffmpeg.av_dict_set(&dictionary, "rtsp_transport", "tcp", 0);

            FormatContext = ffmpeg.avformat_alloc_context();
            var pFormatContext = FormatContext;
            ffmpeg.avformat_open_input(&pFormatContext, url, null, &dictionary);
            ffmpeg.avformat_find_stream_info(FormatContext, null);
            AVCodec* codec = null;

            StreamIndex = ffmpeg
                .av_find_best_stream(FormatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);

            _pPacket = ffmpeg.av_packet_alloc();
            Codec = codec;

        }
        public void Dispose()
        {
            var pPacket = _pPacket;
            ffmpeg.av_packet_free(&pPacket);


            var pFormatContext = FormatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }
        public unsafe void ReadPacket(Queue<AVPacket> aVPackets)
        {
            ffmpeg.av_read_frame(FormatContext, _pPacket);

            aVPackets.Enqueue(*_pPacket);
           // Thread.Sleep(5);
        }
    }
}
