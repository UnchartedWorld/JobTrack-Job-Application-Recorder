using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class AddJobAppViewModel : ViewModelBase
{
    private string? minSalaryInput;
    private string? maxSalaryInput;
    private string? selectedCurrencyChoice;
    private List<string> currencies;

    public List<string> Currencies
    {
        get => currencies;
        set => this.RaiseAndSetIfChanged(ref currencies, value);
    }

    public string? MinSalaryInput
    {
        get => minSalaryInput;
        set => this.RaiseAndSetIfChanged(ref minSalaryInput, value);
    }

    public string? MaxSalaryInput
    {
        get => maxSalaryInput;
        set => this.RaiseAndSetIfChanged(ref maxSalaryInput, value);
    }

    public string? SelectedCurrencyChoice
    {
        get => selectedCurrencyChoice;
        set => this.RaiseAndSetIfChanged(ref selectedCurrencyChoice, value);
    }

    public AddJobAppViewModel()
    {
        this.WhenAnyValue(x => x.MinSalaryInput, x => x.MaxSalaryInput).Subscribe(value =>
        {
            if (value.Item1 != null)
            {
                string viewSalary = FormatInputToNumberString(value.Item1);
                MinSalaryInput = viewSalary;
            }
            else if (value.Item2 != null)
            {
                string viewSalary = FormatInputToNumberString(value.Item2);
                MaxSalaryInput = viewSalary;
            }
        });

        ReadJSONFile();
    }

    /// <summary>
    /// Simply reads a currency JSON file for usage within the application.
    /// </summary>
    public void ReadJSONFile()
    {
        string fullPathToCurrencyJSON = Path.Combine(Environment.CurrentDirectory, "Utils", "currencyData.json");

        if (File.Exists(fullPathToCurrencyJSON))
        {
            string currencyJSONFile = File.ReadAllText(fullPathToCurrencyJSON);
            List<Currency> currencyData = JsonSerializer.Deserialize<List<Currency>>(currencyJSONFile);
            Currencies = currencyData.Select(x => $"{x.CurrencySymbol} - {x.CurrencyCode} ").ToList();
        }
        else
        {
            Console.WriteLine("Something went wrong.");
        }
    }


    /// <summary>
    /// Receives a given input and removes non-numerical values.
    /// </summary>
    /// <param name="inputText">String parameter that represents a given input string.</param>
    /// <returns>Text that is formatted to be only numerical.</returns>
    public string FormatInputToNumberString(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            return string.Empty;
        }
        else
        {
            string formattedText = Regex.Replace(inputText, "[^0-9]", "");

            return formattedText;
        }
    }


}