using System;
using System.Linq;
using System.Windows.Forms;
using NAudio.Core.Wave.WaveOutputs;
using NAudio.Wave.WaveOutputs;

namespace NAudioDemo.AudioPlaybackDemo
{
    class AsioOutPlugin : IOutputDevicePlugin
    {
        AsioOutSettingsPanel settingsPanel;

        public IWavePlayer CreateDevice(int latency)
        {
            return new AsioOut(settingsPanel.SelectedDeviceName);
        }

        public UserControl CreateSettingsPanel()
        {
            settingsPanel = new AsioOutSettingsPanel();
            return settingsPanel;
        }

        public string Name
        {
            get { return "AsioOut"; }
        }

        public bool IsAvailable
        {
            get { return AsioOut.isSupported(); }
        }

        public int Priority
        {
            get { return 4; }
        }
    }
}
