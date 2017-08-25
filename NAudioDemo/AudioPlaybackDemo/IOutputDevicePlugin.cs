using System;
using System.Linq;
using System.Windows.Forms;
using NAudio.Core.Wave.WaveOutputs;

namespace NAudioDemo.AudioPlaybackDemo
{
    public interface IOutputDevicePlugin
    {
        IWavePlayer CreateDevice(int latency);
        UserControl CreateSettingsPanel();
        string Name { get; }
        bool IsAvailable { get; }
        int Priority { get; }
    }
}
