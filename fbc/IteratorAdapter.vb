Public Class IteratorAdapter(Of ILocated)
    Private ReadOnly Enumerator As IEnumerator(Of ILocated)

    Private _HasNext As Boolean
    Private LastValue As ILocated

    Public Sub New(source As IEnumerable(Of ILocated))
        If source Is Nothing Then
            Throw New ArgumentNullException(NameOf(source))
        End If
        Enumerator = source.GetEnumerator()


        Enumerator.MoveNext()
        LastValue = Enumerator.Current
        _HasNext = Enumerator.MoveNext()


    End Sub

    Public ReadOnly Property Current As ILocated
        Get
            Return LastValue
        End Get
    End Property

    Public ReadOnly Property HasNext As Boolean
        Get
            Return _HasNext
        End Get
    End Property

    Public Function [Next]() As ILocated

        If Not HasNext Then
            Throw New InvalidOperationException("No more elements")
        End If

        LastValue = Enumerator.Current
        _HasNext = Enumerator.MoveNext()

        Return LastValue

    End Function

    Public Function NextIfPossible() As ILocated

        If Not HasNext Then
            Return Current
        End If

        LastValue = Enumerator.Current
        _HasNext = Enumerator.MoveNext()

        Return LastValue

    End Function

End Class
