

Imports System.Runtime.InteropServices
Imports System.IO

Public Class frm_main
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
        RegisterHotKey(Me.Handle, 100, MOD_ALT, Keys.S)
        RegisterHotKey(Me.Handle, 200, MOD_ALT, Keys.C)

        timer_delay_app.Start()

    End Sub

    Private Sub timer_delay_app_Tick(sender As Object, e As EventArgs) Handles timer_delay_app.Tick
        'Delay the command line window from starting in order to show the checking... message
        timer_delay_app.Stop()

        'Check if system can execute tishadow run command
        Dim pro = Process.Start("cmd", "/c tishadow run > TishadowAssistantLog.txt")
        pro.WaitForExit()
        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")


        If result.Contains("Beginning Build Process") Then
            'All systems go
        ElseIf Not result.Contains("[ERROR]") Then
            'If user can not get this error then they don't have tishadow installed
            Dim action = MsgBox("Tishadow installation was not detected. Would you like to install it now?" & vbNewLine & vbNewLine & " This can take a few minutes.", vbYesNo + MsgBoxStyle.Exclamation)

            'If they decide to install
            If action = vbYes Then
                Dim install = Process.Start("cmd", "/c npm install -g tishadow > TishadowInstallLog.txt")
                install.WaitForExit()
                Dim logMsg = MsgBox("Tishadow should now be installed. Please re-run Tishadow Asistant. If the install is still not detected, you may have to run this app as administrator." & vbNarrow & vbNewLine & "Would you like to view the log?", MsgBoxStyle.Information + vbYesNo)

                If logMsg = vbYes Then
                    Process.Start("TishadowInstallLog.txt")
                End If

                Application.Restart()
            End If
        ElseIf Not System.IO.File.Exists("tiapp.xml") Then
            'Check if this is a valid Titanium project folder
            MsgBox("Invalid Titanium App project folder detected." & vbNewLine & vbNewLine & _
                   "Place TishadowAssistant.exe in your project's root folder - the one containing tiapp.xml." & vbNewLine & vbNewLine & _
                   "The application will now exit.", MsgBoxStyle.Critical)
            Application.Exit()
        End If

        startupMessage()
    End Sub

    Public Sub startupMessage()
        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Assistant Running For Project:", Application.StartupPath & vbNewLine & vbNewLine & "Press ALT-S to deploy to Tishadow connected devices", ToolTipIcon.Info)
        Me.Hide()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, _
                        ByVal e As System.Windows.Forms.FormClosingEventArgs) _
                        Handles MyBase.FormClosing
        UnregisterHotKey(Me.Handle, 100)
        UnregisterHotKey(Me.Handle, 200)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_HOTKEY Then
            Dim id As IntPtr = m.WParam
            Select Case (id.ToString)
                Case "100"
                    Try
                        Dim pro = Process.Start("cmd", "/c tishadow run > TishadowAssistantLog.txt")
                        pro.WaitForExit()
                        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Assistant [Run]", result, ToolTipIcon.Info)
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Assistant Error", ex.Message, ToolTipIcon.Info)
                    End Try

                Case "200"
                    Try
                        Dim pro = Process.Start("cmd", "/c tishadow clear > TishadowAssistantLog.txt")
                        pro.WaitForExit()
                        Dim result As String = File.ReadAllText("TishadowAssistantLog.txt")
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Assistant [Clear]", result, ToolTipIcon.Info)
                    Catch ex As Exception
                        NotifyIcon1.ShowBalloonTip(1000, "Tishadow Assistant Error", ex.Message, ToolTipIcon.Info)
                    End Try
            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        Dim action = MsgBox("Exit Tishadow Assistant?", MsgBoxStyle.Information + vbYesNo, "Tishadow Assistant")

        If action = vbYes Then
            Application.Exit()
        End If
    End Sub

End Class
