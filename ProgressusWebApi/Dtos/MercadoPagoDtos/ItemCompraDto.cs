namespace ProgressusWebApi.Dtos.MercadoPagoDtos
{
    public class ItemCompraDto
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public int Precio { get; set; }
    }

    public class CompraMercadoPagoDto
    {
        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; }
        public List<ItemCompraDto> ItemsCompra { get; set; }
    }
}
