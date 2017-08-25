﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using NAudio.Core.CoreAudioApi;
using NAudio.Core.MediaFoundation;
using NAudio.Core.Utils;
using NAudio.Utils;
using NAudioWpfDemo.ViewModel;

namespace NAudioWpfDemo.EnumMediaFoundationTransforms
{
    class EnumMftViewModel : ViewModelBase, IDisposable
    {
        public EnumMftViewModel()
        {
            MediaFoundationApi.Startup();
            this.EnumerateCommand = new DelegateCommand(Enumerate);
        }

        public ICommand EnumerateCommand { get; private set; }

        public List<string> Transforms { get; private set; }

        private void Enumerate()
        {
            Transforms = new List<string>();

            var effects = MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioEffect);
            AddTransforms(effects, "Audio Effect");
            AddTransforms(MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioDecoder), "Audio Decoder");
            AddTransforms(MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioEncoder), "Audio Encoder");
            OnPropertyChanged("Transforms");
        }

        private void AddTransforms(IEnumerable<IMFActivate> effects, string type)
        {
            foreach (var mft in effects)
            {
                int attributeCount;
                mft.GetCount(out attributeCount);
                var sb = new StringBuilder();
                sb.AppendFormat(type);
                sb.AppendLine();
                for (int n = 0; n < attributeCount; n++)
                {
                    AddAttribute(mft, n, sb);
                }
                Transforms.Add(sb.ToString());
            }
        }

        private static void AddAttribute(IMFActivate mft, int index, StringBuilder sb)
        {
            var variantPtr = Marshal.AllocHGlobal(MarshalHelpers.SizeOf<PropVariant>());
            try
            {
                Guid key;
                mft.GetItemByIndex(index, out key, variantPtr);
                var value = MarshalHelpers.PtrToStructure<PropVariant>(variantPtr);
                string propertyName = FieldDescriptionHelper.Describe(typeof (MediaFoundationAttributes), key);
                if (key == MediaFoundationAttributes.MFT_INPUT_TYPES_Attributes ||
                    key == MediaFoundationAttributes.MFT_OUTPUT_TYPES_Attributes)
                {
                    var types = value.GetBlobAsArrayOf<MFT_REGISTER_TYPE_INFO>();
                    sb.AppendFormat("{0}: {1} items:", propertyName, types.Length);
                    sb.AppendLine();
                    foreach (var t in types)
                    {
                        sb.AppendFormat("    {0}-{1}",
                            FieldDescriptionHelper.Describe(typeof (MediaTypes), t.guidMajorType),
                            FieldDescriptionHelper.Describe(typeof (AudioSubtypes), t.guidSubtype));
                        sb.AppendLine();
                    }
                }
                else if (key == MediaFoundationAttributes.MF_TRANSFORM_CATEGORY_Attribute)
                {
                    sb.AppendFormat("{0}: {1}", propertyName,
                        FieldDescriptionHelper.Describe(typeof (MediaFoundationTransformCategories), (Guid) value.Value));
                    sb.AppendLine();
                }
                else if (value.DataType == (VarEnum.VT_VECTOR | VarEnum.VT_UI1))
                {
                    var b = (byte[]) value.Value;
                    sb.AppendFormat("{0}: Blob of {1} bytes", propertyName, b.Length);
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendFormat("{0}: {1}", propertyName, value.Value);
                    sb.AppendLine();
                }
            }
            finally
            {
                PropVariant.Clear(variantPtr);
                Marshal.FreeHGlobal(variantPtr);
            }
        }

        public void Dispose()
        {
            MediaFoundationApi.Shutdown();
        }
    }
}