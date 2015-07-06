

Imports System.Runtime.InteropServices
Imports System.IO

Public Class Form1
    'credits
    'https://social.msdn.microsoft.com/Forums/vstudio/en-US/c1a24688-d844-4adc-9d85-416a7158c6ba/faq-how-do-i-register-a-hotkey-in-vbnet


    Public Const MOD_ALT As Integer = &H1 'Alt key
    Public Const WM_HOTKEY As Integer = &H312

    <DllImport("User32.dll")> _
    Public Shared Function RegisterHotKey(ByVal hwnd As IntPtr, _
                        ByVal id As Integer, ByVal fsModifiers As Integer, _
                        ByVal vk As Integer) As Integer
    End Function

    <DllImport("User32.dll")> _
    Public Shared Function UnregisterHotKey(ByVal hwnd As IntPtr, _
                        ByVal id As Integer) As Integer
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, _
                        ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterHotKey(Me.Handle, 100, MOD_ALT, Keys.A)
        RegisterHotKey(Me.Handle, 200, MOD_ALT, Keys.S)
        RegisterHotKey(Me.Handle, 300, MOD_ALT, Keys.D)
        RegisterHotKey(Me.Handle, 400, MOD_ALT, Keys.C)

        NotifyIcon1.ShowBalloonTip(1000, "Note", "Ready", ToolTipIcon.Info)
        Me.Hide()
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_HOTKEY Then
            Dim id As IntPtr = m.WParam
            Select Case (id.ToString)
                Case "100"
                    'MessageBox.Show("You pressed ALT+S key combination")
                    NotifyIcon1.ShowBalloonTip(1000, "Note", "Updating TiShadow Connected Devices", ToolTipIcon.Info)
                    Try
                        WebBrowser1.Refresh()
                        Timer1.Start()
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Error", ex.Message, ToolTipIcon.Info)
                    End Try

                Case "200"
                    Try
                        Dim pro = Process.Start("cmd", "/c tishadow run > TishadowAssistantLog.txt")
                        pro.WaitForExit()
                        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Run Hidden", result, ToolTipIcon.Info)
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Shell Error", ex.Message, ToolTipIcon.Info)
                    End Try

                Case "300"
                    Try
                        Dim pro = Process.Start("cmd", "/k tishadow run > TishadowAssistantLog.txt")
                        pro.WaitForExit()
                        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Run Visible", result, ToolTipIcon.Info)
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Shell Error", ex.Message, ToolTipIcon.Info)
                    End Try

                Case "400"
                    Try
                        Dim pro = Process.Start("cmd", "/c tishadow clear > TishadowAssistantLog.txt")
                        pro.WaitForExit()
                        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Clear", result, ToolTipIcon.Info)
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Shell Error", ex.Message, ToolTipIcon.Info)
                    End Try

            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, _
                        ByVal e As System.Windows.Forms.FormClosingEventArgs) _
                        Handles MyBase.FormClosing
        UnregisterHotKey(Me.Handle, 100)
        UnregisterHotKey(Me.Handle, 200)
    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            WebBrowser1.Document.GetElementById("editor").FirstChild.InnerText = Clipboard.GetText
            Timer1.Stop()
            Timer2.Start()
        Catch ex As Exception
            NotifyIcon1.ShowBalloonTip(1000, "Error", ex.Message, ToolTipIcon.Info)
        End Try
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Try
            WebBrowser1.Document.GetElementById("tisubmit").InvokeMember("click")
            Timer2.Stop()
        Catch ex As Exception
            NotifyIcon1.ShowBalloonTip(1000, "Error", ex.Message, ToolTipIcon.Info)
        End Try
    End Sub


    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Application.Restart()
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        Me.WindowState = FormWindowState.Maximized
        Me.Show()
        Me.WindowState = FormWindowState.Maximized
    End Sub
    Private Sub NotifyIcon1_BalloonTipClicked(sender As Object, e As EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Me.WindowState = FormWindowState.Maximized
        Me.Show()
        Me.WindowState = FormWindowState.Maximized
    End Sub
End Class
