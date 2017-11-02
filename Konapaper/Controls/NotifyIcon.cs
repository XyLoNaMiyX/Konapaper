// Class intended for WPF use
// Made by Lonami Exo
// Creation date: december 2015
// Last edit:     march    2016
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

class NotifyIcon
{
    #region Events

    public delegate void MouseEventHandler(object sender, MouseButton button);

    public event MouseEventHandler MouseDown;
    public event MouseEventHandler MouseUp;

    #endregion

    #region Private variables

    System.Windows.Forms.NotifyIcon notifyIcon;

    #endregion

    #region Public properties

    public Uri Icon
    {
        set
        {
            notifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(value).Stream,
                System.Windows.Forms.SystemInformation.SmallIconSize);

            /*

                   MemoryStream memStream = new MemoryStream();              
        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(imageC));
        encoder.Save(memStream);
        return memStream.ToArray();

            */
        }
    }
    public string Text
    {
        get { return notifyIcon.Text; }
        set { notifyIcon.Text = value; }
    }
    public bool Visible
    {
        get { return notifyIcon.Visible; }
        set { notifyIcon.Visible = value; }
    }

    #endregion

    #region Constructor

    public NotifyIcon(Uri icon, string text)
    {
        notifyIcon = new System.Windows.Forms.NotifyIcon();
        Icon = icon;
        Text = text;

        notifyIcon.MouseDown += NotifyIcon_MouseDown;
        notifyIcon.MouseUp += NotifyIcon_MouseUp;
    }

    #endregion

    #region Context menu

    public void AddContextMenuItem(string caption, Action action)
    {
        if (notifyIcon.ContextMenu == null)
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

        notifyIcon.ContextMenu.MenuItems.Add(caption, (s, e) => action());
    }

    public void ClearContextMenuItems()
    {
        if (notifyIcon.ContextMenu != null)
            notifyIcon.ContextMenu.MenuItems.Clear();
    }

    #endregion

    #region Handle mouse events

    void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (MouseDown != null)
            MouseDown(this, getButton(e));
    }

    void NotifyIcon_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (MouseUp != null)
            MouseUp(this, getButton(e));
    }

    MouseButton getButton(System.Windows.Forms.MouseEventArgs e)
    {
        switch (e.Button)
        {
            default:
            case System.Windows.Forms.MouseButtons.Left:
                return MouseButton.Left;
            case System.Windows.Forms.MouseButtons.Middle:
                return MouseButton.Middle;
            case System.Windows.Forms.MouseButtons.Right:
                return MouseButton.Right;
            case System.Windows.Forms.MouseButtons.XButton1:
                return MouseButton.XButton1;
            case System.Windows.Forms.MouseButtons.XButton2:
                return MouseButton.XButton2;
        }
    }

    #endregion
}