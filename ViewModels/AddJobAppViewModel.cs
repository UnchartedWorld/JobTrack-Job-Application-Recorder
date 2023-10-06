using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class AddJobAppViewModel : ValidatingViewModel
{
    #region Fields
    private string? companyName;
    private string? jobTitle;
    private string? jobURL;
    private string? minSalaryInput;
    private string? maxSalaryInput;
    private string? jobLocation;
    private DateTimeOffset? dateOfApplication;
    private string? generalNotes;

    private string? selectedCurrencyChoice;
    private string? selectedJobFlex;
    private List<string> currencies;

    #endregion

    #region Properties
    public bool ToggleSubmitBtn => !HasErrors;

    public List<string> Currencies
    {
        get => currencies;
        set => this.RaiseAndSetIfChanged(ref currencies, value);
    }

    public List<string> JobFlexOptions { get; } = new List<string> { "In-Office", "Hybrid", "Remote" };

    public string? CompanyName
    {
        get => companyName;
        set => this.RaiseAndSetIfChanged(ref companyName, value);
    }

    public string? JobTitle
    {
        get => jobTitle;
        set => this.RaiseAndSetIfChanged(ref jobTitle, value);
    }

    public string? JobURL
    {
        get => jobURL;
        set => this.RaiseAndSetIfChanged(ref jobURL, value);
    }

    public string? JobLocation
    {
        get => jobLocation;
        set => this.RaiseAndSetIfChanged(ref jobLocation, value);
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

    public string? SelectedJobFlex
    {
        get => selectedJobFlex;
        set => this.RaiseAndSetIfChanged(ref selectedJobFlex, value);
    }

    public DateTimeOffset? DateOfApplication
    {
        get => dateOfApplication;
        set => this.RaiseAndSetIfChanged(ref dateOfApplication, value);
    }

    public string? GeneralNotes
    {
        get => generalNotes;
        set => this.RaiseAndSetIfChanged(ref generalNotes, value);
    }

    #endregion

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

        // I'm not entirely sure why, but this is entirely necessary for HasErrors to be tracked
        // in this ViewModel. Without it, it's unable to reflect updates. It should be noted that
        // .ToProperty isn't updating the value, but I need it here for things to work. Strange.
        this.WhenAnyValue(x => x.HasErrors).ToProperty(this, x => x.ToggleSubmitBtn);

        // Adds all desired validation checks for each input field.

        this.WhenAnyValue(vm => vm.SelectedCurrencyChoice).Subscribe(_ => ValidateCurrencyInput());
        this.WhenAnyValue(vm => vm.CompanyName).Subscribe(_ => ValidateCompanyName());
        this.WhenAnyValue(vm => vm.JobTitle).Subscribe(_ => ValidateJobTitle());
        this.WhenAnyValue(vm => vm.JobURL).Subscribe(_ => ValidateJobURL());
        this.WhenAnyValue(vm => vm.JobLocation).Subscribe(_ => ValidateJobLocation());
        this.WhenAnyValue(vm => vm.MinSalaryInput).Subscribe(_ => ValidateMinSalary());
        this.WhenAnyValue(vm => vm.MaxSalaryInput).Subscribe(_ => ValidateMaxSalary());
        this.WhenAnyValue(vm => vm.SelectedJobFlex).Subscribe(_ => ValidateJobFlex());
        this.WhenAnyValue(vm => vm.DateOfApplication).Subscribe(_ => ValidateJobDate());

        ReadJSONFile();
    }

    #region Methods

    public void ValidateCurrencyInput()
    {
        ClearErrors(nameof(SelectedCurrencyChoice));

        if (string.IsNullOrWhiteSpace(SelectedCurrencyChoice))
        {
            AddError(nameof(SelectedCurrencyChoice), "This field is required.");
        }
        else if (SelectedCurrencyChoice.Length > 8)
        {
            AddError(nameof(SelectedCurrencyChoice), "Input is too long, remove excess whitespace.");
        }
        else if (!Currencies.Contains(SelectedCurrencyChoice))
        {
            AddError(nameof(SelectedCurrencyChoice), "Currency isn't present in system.");
        }
        else
        {
            ClearErrors(nameof(SelectedCurrencyChoice));
        }
    }

    public void ValidateCompanyName()
    {
        ClearErrors(nameof(CompanyName));

        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            AddError(nameof(CompanyName), "This field is required.");
        }
        else
        {
            ClearErrors(nameof(CompanyName));
        }
    }

    public void ValidateJobTitle()
    {
        ClearErrors(nameof(JobTitle));

        if (string.IsNullOrWhiteSpace(JobTitle))
        {
            AddError(nameof(JobTitle), "This field is required.");
        }
        else
        {
            ClearErrors(nameof(JobTitle));
        }
    }

    public void ValidateJobURL()
    {
        ClearErrors(nameof(JobURL));

        if (string.IsNullOrWhiteSpace(JobURL))
        {
            AddError(nameof(JobURL), "This field is required.");
        }
        else
        {
            ClearErrors(nameof(JobURL));
        }
    }

    public void ValidateJobLocation()
    {
        ClearErrors(nameof(JobLocation));

        if (string.IsNullOrWhiteSpace(JobLocation))
        {
            AddError(nameof(JobLocation), "This field is required.");
        }
        else
        {
            ClearErrors(nameof(JobLocation));
        }
    }

    public void ValidateJobFlex()
    {
        ClearErrors(nameof(SelectedJobFlex));

        if (string.IsNullOrEmpty(SelectedJobFlex))
        {
            AddError(nameof(SelectedJobFlex), "This field is required.");
        }
        else
        {
            ClearErrors(nameof(SelectedJobFlex));
        }
    }

    public void ValidateMinSalary()
    {
        ClearErrors(nameof(MinSalaryInput));

        Regex numRegex = new(@"[^0-9]");

        if (int.TryParse(MinSalaryInput, out int val1) && int.TryParse(MaxSalaryInput, out int val2))
        {
            if (val1 > val2)
            {
                AddError(nameof(MinSalaryInput), "The minimum salary cannot be higher than the maximum.");
            }
        }
        else if (string.IsNullOrWhiteSpace(MinSalaryInput))
        {
            AddError(nameof(MinSalaryInput), "This field is required.");
        }
        else if (numRegex.IsMatch(MinSalaryInput))
        {
            AddError(nameof(MinSalaryInput), "Detected non-numerical inputs, please only enter numbers.");
        }
        else
        {
            ClearErrors(nameof(MinSalaryInput));
        }
    }

    public void ValidateMaxSalary()
    {
        ClearErrors(nameof(MaxSalaryInput));

        Regex numRegex = new(@"[^0-9]");

        if (int.TryParse(MinSalaryInput, out int val1) && int.TryParse(MaxSalaryInput, out int val2))
        {
            if (val2 < val1)
            {
                AddError(nameof(MinSalaryInput), "The minimum salary cannot be higher than the maximum.");
            }
        }
        else if (string.IsNullOrWhiteSpace(MaxSalaryInput))
        {
            AddError(nameof(MaxSalaryInput), "This field is required.");
        }
        else if (numRegex.IsMatch(MaxSalaryInput))
        {
            AddError(nameof(MaxSalaryInput), "Detected non-numerical inputs, please only enter numbers.");
        }
        else
        {
            ClearErrors(nameof(MaxSalaryInput));
        }
    }

    public void ValidateJobDate()
    {
        ClearErrors(nameof(DateOfApplication));

        if (DateOfApplication.Equals(null))
        {
            AddError(nameof(DateOfApplication), "No date of applying to job given. Please select one.");
        }
        else
        {
            ClearErrors(nameof(DateOfApplication));
        }
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
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return string.Empty;
        }
        else
        {
            string formattedText = Regex.Replace(inputText, "[^0-9]", "");

            return formattedText;
        }
    }

    #endregion


}