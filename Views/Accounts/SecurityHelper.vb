Imports System
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq

Public Class SecurityHelper
    Private pass As Byte(), keyBytes As Byte()
    Private len As Integer

    Private ReadOnly rij As Aes = Aes.Create("AesManaged")

    Public Shared ReadOnly Secure As New SecurityHelper()

    Private Sub New()
        With rij
            .Mode = CipherMode.CBC
            .Padding = PaddingMode.PKCS7
            .KeySize = &H80
            .BlockSize = &H80
        End With
        pass = Encoding.UTF8.GetBytes(JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetParent(Startup.GetEnv.WebRootPath).ToString(), "appsettings.json")))!key.ToString())
    End Sub

    Public Function Encrypt(text As String) As String
        keyBytes = Enumerable.Repeat(&H0, 16).Select(AddressOf Convert.ToByte).ToArray()
        len = pass.Length
        If len > keyBytes.Length Then len = keyBytes.Length
        Array.Copy(pass, keyBytes, len)
        With rij
            .Key = keyBytes
            .IV = keyBytes
            Dim textDataByte As Byte() = Encoding.UTF8.GetBytes(text)
            Return Convert.ToBase64String(.CreateEncryptor().TransformFinalBlock(textDataByte, 0, textDataByte.Length))
        End With
    End Function

    Public Function Decrypt(text As String) As String
        Dim encrypted As Byte() = Convert.FromBase64String(text)
        keyBytes(&HF) = New Byte()
        len = pass.Length
        If len > keyBytes.Length Then len = keyBytes.Length
        Array.Copy(pass, keyBytes, len)
        With rij
            .Key = keyBytes
            .IV = keyBytes
            Return Encoding.UTF8.GetString(.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length))
        End With
    End Function
End Class