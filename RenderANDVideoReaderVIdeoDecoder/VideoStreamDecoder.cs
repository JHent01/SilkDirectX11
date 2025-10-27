using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public sealed unsafe class VideoStreamDecoder : IDisposable
    {
        private readonly AVCodecContext* _pCodecContext;
        private readonly AVFrame* _pFrame;
        private readonly AVFrame* _receivedFrame;
        private readonly AVFormatContext* _formatContext;
        private int _streamIndex;


        public VideoStreamDecoder(VideoReader reader)
        {
            AVHWDeviceType HWDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA; // ; //AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA;


            _formatContext = reader.FormatContext;
            _pCodecContext = ffmpeg.avcodec_alloc_context3(reader.Codec);
            _streamIndex = reader.StreamIndex;


            if (HWDeviceType != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
            {
                ffmpeg.av_hwdevice_ctx_create(&_pCodecContext->hw_device_ctx, HWDeviceType, null, null, 0)
                     ;
            }
            ffmpeg.avcodec_parameters_to_context(_pCodecContext, reader.FormatContext->streams[_streamIndex]->codecpar);
            ffmpeg.avcodec_open2(_pCodecContext, reader.Codec, null);

            CodecName = ffmpeg.avcodec_get_name(reader.Codec->id);
            FrameSize = new Size(_pCodecContext->width, _pCodecContext->height);
            PixelFormat = _pCodecContext->pix_fmt;


            _pFrame = ffmpeg.av_frame_alloc();
            _receivedFrame = ffmpeg.av_frame_alloc();
        }
        public string CodecName { get; }
        public Size FrameSize { get; }
        public AVPixelFormat PixelFormat { get; }

        public void Dispose()
        {
            var pFrame = _pFrame;
            ffmpeg.av_frame_free(&pFrame);



            ffmpeg.avcodec_close(_pCodecContext);

        }

        public bool TryDecodeNextFrame(out AVFrame frame, AVPacket aVPacket)
        {
            int error;
            ffmpeg.av_frame_unref(_receivedFrame);
            ffmpeg.avcodec_send_packet(_pCodecContext, &aVPacket);
            error = ffmpeg.avcodec_receive_frame(_pCodecContext, _pFrame);
            if (error == ffmpeg.AVERROR(ffmpeg.EAGAIN)) { frame = *_pFrame; return false; }
            if (_pCodecContext->hw_device_ctx != null)
            {
                ffmpeg.av_hwframe_transfer_data(_receivedFrame, _pFrame, 0);
                frame = *_receivedFrame;
                ffmpeg.av_packet_unref(&aVPacket);
                ffmpeg.av_frame_unref(_pFrame);

                return true;
            }


            if (error == ffmpeg.AVERROR(ffmpeg.EAGAIN)) { frame = *_pFrame; return false; }
            frame = *_pFrame;
            ffmpeg.av_packet_unref(&aVPacket);
            ffmpeg.av_frame_unref(_pFrame);
            return true;


        }


    }
}
