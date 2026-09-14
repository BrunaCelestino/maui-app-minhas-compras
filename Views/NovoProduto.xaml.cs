using MauiAppMinhasCompras.Constants;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
    public NovoProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            picker_categoria.ItemsSource = Categorias.Lista;
            Produto p = new Produto
            {
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                Categoria = picker_categoria.SelectedItem?.ToString()
            };

            await App.Db.Insert(p);
            await DisplayAlert("Sucesso!", "Registro Inserido", "OK");

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        picker_categoria.ItemsSource = Categorias.Lista;

        if (BindingContext is Produto produto)
        {
            picker_categoria.SelectedItem = produto.Categoria;
        }
    }
}