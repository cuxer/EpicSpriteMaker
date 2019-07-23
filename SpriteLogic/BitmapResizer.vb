Option Explicit On
Option Strict On
Option Infer Off
Public Class BitmapResizer
    Private ReadOnly _listOfBitmaps As List(Of Drawing.Bitmap)

#Region "Class Busywork"
    Public Sub New()
        _listOfBitmaps = New List(Of Drawing.Bitmap)
    End Sub

    Public Sub AddBitmap(ByRef bitmap As Drawing.Bitmap)
        _listOfBitmaps.Append(bitmap)
    End Sub

    Public Sub AddImage(ByRef image As Drawing.Image)
        Dim bitmap As Drawing.Bitmap = New Drawing.Bitmap(image)
        'We want to store bitmaps because they are easier to work with.
        _listOfBitmaps.Append(bitmap)
    End Sub

    Public Sub ClearBitmaps()
        _listOfBitmaps.Clear()
    End Sub

    Public Sub RemoveBitmap(ByRef bitmap As Drawing.Bitmap)
        If _listOfBitmaps.Contains(bitmap) Then
            _listOfBitmaps.Remove(bitmap)
        End If
    End Sub

    Public Sub RemoveBitmapAt(ByVal index As Integer)
        If index > 0 AndAlso index <= _listOfBitmaps.Count() - 1 Then
            _listOfBitmaps.RemoveAt(index)
        Else
            Throw New IndexOutOfRangeException("Range must be within the bounds of the list of images.")
        End If
    End Sub
#End Region

#Region "Logic"
    Public Sub ResizeAll(ByVal targetWidth As Int32, ByVal targetHeight As Int32)
        Dim bitmap As Drawing.Bitmap
        Dim indexOfCurrentBitmap As Integer
        Dim modifiedBitmap As Drawing.Bitmap
        For Each bitmap In _listOfBitmaps
            indexOfCurrentBitmap = _listOfBitmaps.IndexOf(bitmap)
            modifiedBitmap = BitmapResizer.ResizeImage(bitmap, targetWidth, targetHeight)
            _listOfBitmaps.RemoveAt(indexOfCurrentBitmap)
            _listOfBitmaps.Insert(indexOfCurrentBitmap, modifiedBitmap)
        Next bitmap
    End Sub

    Public Overloads Shared Function ResizeImage(ByRef sourceImage As Drawing.Image, ByVal targetWidth As Int32, ByVal targetHeight As Int32) As Drawing.Bitmap
        Dim bmSource As Drawing.Bitmap = New Drawing.Bitmap(sourceImage)

        Return ResizeImage(bmSource, targetWidth, targetHeight)
    End Function

    Public Overloads Shared Function ResizeImage(ByRef bmSource As Drawing.Bitmap, ByVal targetWidth As Int32, ByVal targetHeight As Int32) As Drawing.Bitmap
        Dim bmDest As New Drawing.Bitmap(targetWidth, targetHeight, Drawing.Imaging.PixelFormat.Format32bppArgb)

        Dim sourceAspectRatio As Double = bmSource.Width / bmSource.Height
        Dim destAspectRatio As Double = bmDest.Width / bmDest.Height

        Dim newX As Int32 = 0
        Dim newY As Int32 = 0
        Dim newWidth As Int32 = bmDest.Width
        Dim newHeight As Int32 = bmDest.Height

        If Equals(destAspectRatio, sourceAspectRatio) Then
            'same ratio
        ElseIf destAspectRatio > sourceAspectRatio Then
            'Source is taller
            newWidth = Convert.ToInt32(Math.Floor(sourceAspectRatio * newHeight))
            newX = Convert.ToInt32(Math.Floor((bmDest.Width - newWidth) / 2))
        Else
            'Source is wider
            newHeight = Convert.ToInt32(Math.Floor((1 / destAspectRatio) * newWidth))
            newY = Convert.ToInt32(Math.Floor((bmDest.Height - newHeight) / 2))
        End If

        Using grDest As Drawing.Graphics = Drawing.Graphics.FromImage(bmDest)
            With grDest
                .CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                .InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                .PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
                .SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
                .CompositingMode = Drawing.Drawing2D.CompositingMode.SourceOver

                .DrawImage(bmSource, newX, newY, newHeight, newHeight)
            End With
        End Using

        Return bmDest
    End Function
#End Region
End Class