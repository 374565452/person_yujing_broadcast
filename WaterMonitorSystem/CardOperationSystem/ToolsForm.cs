using System;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public class ToolsForm
    {
        public static void AddLog(string str, bool isNewLineBegin, bool isNewLineEnd, TextBox t)
        {
            if (isNewLineBegin)
                t.Text += Environment.NewLine;

            t.Text += str;

            if (isNewLineEnd)
                t.Text += Environment.NewLine;

            t.Select(t.TextLength, 0);//光标定位到文本最后
            t.ScrollToCaret();
        }

        public static void setTextLabel(Label l, string s, string defaultValue, int type)
        {
            l.Text = s.TrimStart('0');
            if (l.Text == "") l.Text = defaultValue;
            else
            {
                if (type == 1)
                    l.Text = (double.Parse(l.Text) / 10.0).ToString("0.0");
            }
        }

        public static void setTextTextBox(TextBox t, string s, string defaultValue, int type)
        {
            try
            {
                t.Text = s.TrimStart('0');
                if (t.Text == "") t.Text = defaultValue;
                else
                {
                    if (type == 1)
                        t.Text = (double.Parse(t.Text) / 10.0).ToString("0.0");
                }
            }
            catch (Exception ex)
            {
                t.Text = defaultValue;
            }
        }
    }
}
