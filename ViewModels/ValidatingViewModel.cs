using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public abstract class ValidatingViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private Dictionary<string, List<ValidationResult>> validationErrors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => validationErrors.Count > 0;

    /// <inheritdoc />
    public IEnumerable GetErrors(string? propName)
    {
        // Get entity-level errors when the target property is null or empty
        if (string.IsNullOrEmpty(propName))
            return validationErrors.Values;

        // Property-level errors, if any
        if (validationErrors.TryGetValue(propName, out var errors))
            return errors;

        // In case there are no errors we return an empty array.
        return Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Clears the errors for a given property name.
    /// </summary>
    /// <param name="propertyName">The name of the property to clear or all properties if <see langword="null"/></param>
    protected void ClearErrors(string? propertyName = null)
    {
        // Clear entity-level errors when the target property is null or empty
        if (string.IsNullOrEmpty(propertyName))
        {
            validationErrors.Clear();
        }
        else
        {
            validationErrors.Remove(propertyName);
        }

        // Notify that errors have changed
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        this.RaisePropertyChanged(nameof(HasErrors));
    }

    /// <summary>
    /// Adds a given error message for a given property name.
    /// </summary>
    /// <param name="propertyName">the name of the property</param>
    /// <param name="errorMessage">The error message to show</param>
    protected void AddError(string propertyName, string errorMessage)
    {
        // Add the cached errors list for later use.
        if (!validationErrors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
        {
            propertyErrors = new List<ValidationResult>();
            validationErrors.Add(propertyName, propertyErrors);
        }

        propertyErrors.Add(new ValidationResult(errorMessage));

        // Notify that errors have changed
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        this.RaisePropertyChanged(nameof(HasErrors));
    }
}