Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Services
Imports WebApi.Models

' Web Service SOAP clasico (ASMX).
' Pagina de prueba: /Services/ProductosService.asmx
' Contrato WSDL:     /Services/ProductosService.asmx?wsdl
<WebService(Namespace:="http://tempuri.org/WebApi/ProductosService")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class ProductosService
    Inherits System.Web.Services.WebService

    ' Almacen en memoria compartido entre peticiones (Shared). El SyncLock
    ' evita condiciones de carrera porque ASMX crea una instancia por peticion.
    Private Shared ReadOnly _candado As New Object()
    Private Shared ReadOnly _productos As New List(Of Producto) From {
        New Producto With {.Id = 1, .Nombre = "Teclado", .Precio = 75000D},
        New Producto With {.Id = 2, .Nombre = "Mouse", .Precio = 45000D},
        New Producto With {.Id = 3, .Nombre = "Monitor", .Precio = 650000D}
    }
    Private Shared _siguienteId As Integer = 4

    <WebMethod(Description:="Devuelve la lista completa de productos.")>
    Public Function ObtenerProductos() As List(Of Producto)
        SyncLock _candado
            Return _productos.ToList()
        End SyncLock
    End Function

    <WebMethod(Description:="Agrega un producto y devuelve el registro creado con su Id asignado.")>
    Public Function AgregarProducto(ByVal nombre As String, ByVal precio As Decimal) As Producto
        SyncLock _candado
            Dim nuevo As New Producto With {
                .Id = _siguienteId,
                .Nombre = nombre,
                .Precio = precio
            }
            _siguienteId += 1
            _productos.Add(nuevo)
            Return nuevo
        End SyncLock
    End Function

    <WebMethod(Description:="Elimina un producto por Id. Devuelve True si se elimino, False si no existia.")>
    Public Function EliminarProducto(ByVal id As Integer) As Boolean
        SyncLock _candado
            Dim producto = _productos.FirstOrDefault(Function(p) p.Id = id)
            If producto Is Nothing Then
                Return False
            End If
            _productos.Remove(producto)
            Return True
        End SyncLock
    End Function
End Class
