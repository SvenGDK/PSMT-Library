Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging
Imports Newtonsoft.Json

Public Class PSNInfo

    Public CurrentGameContentID As String = String.Empty
    Dim WithEvents PSNBrowser As New Forms.WebBrowser() With {.ScriptErrorsSuppressed = True}

    Private Sub PSNInfo_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not String.IsNullOrEmpty(CurrentGameContentID) Then
            PSNBrowser.Navigate("https://store.playstation.com/product/" + CurrentGameContentID)
        End If
    End Sub

    Private Sub PSNBrowser_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles PSNBrowser.DocumentCompleted
        If PSNBrowser.Document.GetElementById("mfe-jsonld-tags") IsNot Nothing Then
            If Not String.IsNullOrEmpty(PSNBrowser.Document.GetElementById("mfe-jsonld-tags").InnerHtml) Then
                Dim JSONData = PSNBrowser.Document.GetElementById("mfe-jsonld-tags").InnerHtml
                Dim StoreInfos = JsonConvert.DeserializeObject(Of Structures.StorePageInfos)(JSONData)

                If Not String.IsNullOrEmpty(StoreInfos.name) Then
                    GameTitleTextBlock.Text = StoreInfos.name
                End If
                If Not String.IsNullOrEmpty(StoreInfos.description) Then
                    DescriptionTextBlock.Text = StoreInfos.description
                End If
                If Not String.IsNullOrEmpty(StoreInfos.category) Then
                    CategoryTextBlock.Text = StoreInfos.category
                End If
                If Not String.IsNullOrEmpty(StoreInfos.sku) Then
                    GameCodeTextBlock.Text = StoreInfos.sku
                End If
                If Not String.IsNullOrEmpty(StoreInfos.image) Then
                    GameImage.Source = New BitmapImage(New Uri(StoreInfos.image, UriKind.RelativeOrAbsolute))
                End If
            End If
        End If
    End Sub

End Class
