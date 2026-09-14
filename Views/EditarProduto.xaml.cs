using MauiAppMinhasCompras.Constants;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    public EditarProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;
            picker_categoria.ItemsSource = Categorias.Lista;


            Produto p = new Produto
            {
                Id = produto_anexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                Categoria = picker_categoria.SelectedItem?.ToString()
            };

            await App.Db.Update(p);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();
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