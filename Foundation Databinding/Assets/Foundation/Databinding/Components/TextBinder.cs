// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using Foundation.Databinding.View;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// UIText to String
    /// </summary>
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Foundation/Databinding/TextBinder")]
    public class TextBinder : BindingBase
    {
        protected Text Label;

        [HideInInspector]
        public BindingInfo LabelBinding = new BindingInfo { BindingName = "Label" };
        [HideInInspector]
        public BindingInfo ColorBinding = new BindingInfo { BindingName = "Color" };

        public string FormatString = string.Empty;


        protected bool IsInit;

        protected void Awake()
        {
            Init();
        }

        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            Label = GetComponentInChildren<Text>();

            if (Label == null)
                Label = GetComponent<Text>();

            if (Label == null)
                Debug.Log("Text Not Found", gameObject);

            LabelBinding.Action = UpdateLabel;
            LabelBinding.Filters = BindingFilter.Properties;

            ColorBinding.Action = UpdateColor;
            ColorBinding.Filters = BindingFilter.Properties;
            ColorBinding.FilterTypes = new[] { typeof(Color) };
        }


        private void UpdateLabel(object arg)
        {
            var s = arg == null ? string.Empty : arg.ToString();

            if (Label)
            {
                if (string.IsNullOrEmpty(FormatString))
                {
                    if (DebugMode)
                        Debug.Log(FormatString);
                    Label.text = s;
                }
                else
                {
                    Label.text = string.Format(FormatString, arg);
                }
            }
        }

        private void UpdateColor(object arg)
        {
            if (Label)
            {
                Label.color = ((Color)arg);
            }
        }

    }
}
