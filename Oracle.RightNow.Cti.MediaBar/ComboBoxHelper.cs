using Oracle.RightNow.Cti.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Oracle.RightNow.Cti.MediaBar
{
    public class ComboBoxHelper
    {
        /// <summary>
        /// Find if the Combobox content is trimmed or not.
        /// </summary>
        public static readonly DependencyPropertyKey IsTextTrimmedKey = DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(ComboBoxHelper), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static Boolean GetIsTextTrimmed(ComboBox target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        /// <summary>
        /// Allow combobox to use the selection changed event for calculating the content.
        /// </summary>
        public static DependencyProperty ListenSelectionChangedProperty = DependencyProperty.RegisterAttached("ListenSelectionChanged", typeof(bool), typeof(ComboBoxHelper), new UIPropertyMetadata(ComboBoxHelper.SelectedItemChanged));


        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetListenSelectionChanged(DependencyObject target, bool value)
        {
            target.SetValue(ComboBoxHelper.ListenSelectionChangedProperty, value);
        }

        private static void SelectedItemChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Selector element = target as Selector;
            if (element == null) return;
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                element.SelectionChanged += SelectionChanged;
            }
            else
            {
                element.SelectionChanged -= SelectionChanged;
            }
        }

        private static void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (null == comboBox)
            {
                return;
            }
            comboBox.SetValue(IsTextTrimmedKey, CalculateIsTextTrimmed(comboBox));
        }

        /// <summary>
        /// Calculate the content of combo box is trimmed or not.
        /// </summary>
        /// <param name="comboBox">calculate the content of the combobox.</param>
        /// <returns>return true,if text is trimmed else false.</returns>
        private static bool CalculateIsTextTrimmed(ComboBox comboBox)
        {
            Typeface typeface = new Typeface(
              comboBox.FontFamily,
              comboBox.FontStyle,
              comboBox.FontWeight,
              comboBox.FontStretch);

            if (comboBox == null)
                return false;

            FormattedText formattedText = null;

            AgentState agentStatusTemplate = comboBox.SelectedItem as AgentState;
            if (agentStatusTemplate != null)
            {
                // FormattedText is used to measure the whole width of the text held up by ComboBox container
                formattedText = new FormattedText(
                    agentStatusTemplate.Name+Environment.NewLine+agentStatusTemplate.Description,
                    System.Threading.Thread.CurrentThread.CurrentCulture,
                    comboBox.FlowDirection,
                    typeface,
                    comboBox.FontSize,
                    comboBox.Foreground);
            }

            

            if (formattedText == null)
                return false;


            /*MaxTextWidth = 140 ,the content of the combo box width is 160.if ellipsis happened the width will be 140.
              so only 140 has been set. */
            formattedText.MaxTextWidth = 140;

            // When the maximum text width of the FormattedText instance is set to the actual
            // width of the combobox, if the combobox is being trimmed to fit then the formatted
            // text will report a larger height than the combobox. Should work whether the
            // combobox is single or multi-line.
            return (formattedText.Height > comboBox.ActualHeight);
        }
    }
}
