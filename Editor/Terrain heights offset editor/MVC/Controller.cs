
/// <summary>
/// This class represents tool's business-logic between <see cref="Model"/> and <see cref="View"/>
/// </summary>
public class Controller
{
    /// <summary>
    /// Model component of the MVC
    /// </summary>
    private Model _model;

    /// <summary>
    /// View component of the MVC
    /// </summary>
    private View _view;

    /// <summary>
    /// Window with warning dialog
    /// </summary>
    private WarningDialog _dialog;

    public Controller(Model model, View view)
    {
        _model = model;
        _view = view;

        _view.ApplyButtonPressed += OnApplyButtonPressed;
        _view.WindowDestroyed += OnWindowDestroyed;
    }

    /// <summary>
    /// Performs actions related to <see cref="_view"/> disposal
    /// </summary>
    private void OnWindowDestroyed()
    {
        _view.ApplyButtonPressed -= OnApplyButtonPressed;

        if (_dialog != null)
            _dialog.Proceed -= OnWarningDialogProceed;
    }

    /// <summary>
    /// Performs actions related to apply button press
    /// </summary>
    private void OnApplyButtonPressed()
    {
        _model.UpdateData(_view);

        if (!_model.AreHeightsOffsetable(out string message))
        {
            if (_dialog == null)
            {
                _dialog = new WarningDialog(message);
            }
            _dialog.Proceed += OnWarningDialogProceed;
            _dialog.ShowModalUtility();
        }
        else
        {
            _model.ApplyOffset();
        }
    }

    /// <summary>
    /// Performs actions related to positive dialog result in <see cref="_dialog"/>
    /// </summary>
    private void OnWarningDialogProceed()
    {
        _dialog.Proceed -= OnWarningDialogProceed;

        _model.ApplyOffset();
    }
}
