using MauiAppMinhasCompras.Constants;
using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();

        lst_produtos.ItemsSource = lista;

        picker_categoria.ItemsSource =
            new List<string> { "Todas" }
            .Concat(Categorias.Lista)
            .ToList();

        picker_categoria.SelectedIndex = 0;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await CarregarProdutos();
    }

    private async Task CarregarProdutos()
    {
        try
        {
            List<Produto> produtos = await App.Db.GetAll();

            lista.Clear();

            foreach (Produto produto in produtos)
            {
                lista.Add(produto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        await AplicarFiltros();
    }

    private async void picker_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        await AplicarFiltros();
    }

    private async Task AplicarFiltros()
    {
        try
        {
            string pesquisa = txt_search.Text?.Trim() ?? "";
            string categoria = picker_categoria.SelectedItem?.ToString();

            List<Produto> produtos = await App.Db.GetAll();

            if (!string.IsNullOrEmpty(categoria) && categoria != "Todas")
            {
                produtos = produtos
                    .Where(p =>
                        !string.IsNullOrEmpty(p.Categoria) &&
                        p.Categoria.Equals(
                            categoria,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(pesquisa))
            {
                produtos = produtos
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Descricao) &&
                         p.Descricao.Contains(
                             pesquisa,
                             StringComparison.OrdinalIgnoreCase))
                        ||
                        (!string.IsNullOrEmpty(p.Categoria) &&
                         p.Categoria.Contains(
                             pesquisa,
                             StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }

            lista.Clear();

            foreach (Produto produto in produtos)
            {
                lista.Add(produto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        await DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void ToolbarItem_Clicked_relatorio(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        var totalPorCategoria = lista
            .GroupBy(p => p.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                ValorTotal = g.Sum(p => p.Total)
            })
            .ToList();

        string msg = $"Total Geral: R$ {soma:F2}\n\n";
        msg += "Total por Categoria:\n";

        foreach (var categoria in totalPorCategoria)
        {
            msg += $"{categoria.Categoria}: R$ {categoria.ValorTotal:F2}\n";
        }

        await DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?",
                $"Remover {p.Descricao}?",
                "Sim",
                "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);

                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_ItemSelected(
        object sender,
        SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem == null)
                return;

            Produto p = e.SelectedItem as Produto;

            await Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });

            lst_produtos.SelectedItem = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}