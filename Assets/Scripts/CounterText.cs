using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CounterText : MonoBehaviour
{
    [SerializeField] private double _minChangeSpeed = 20f;

    private double _duration = 0.5f;
    private double _prevDisplayedValue;
    private double _targetValue;
    private double _currentValue;
    private double _changeSpeed;
    private double _displayedValue;
    
    public delegate void ChangeHandler(double value, double change);
    private ChangeHandler _onValueChanged;
    public delegate void CompleteHandler();
    private CompleteHandler _onComplete;
    public delegate string CustomFormatter(double value);
    private CustomFormatter _customFormatter;

    private string _formatString;
    private string _formatStringKey;

    private bool _initialized;
    private bool _completeHandled = true;

    private TextMeshProUGUI _text;

    private Type _type;

    private Material _defaultTextSharedMaterial;

    public double Duration => _duration;

    private void Awake()
    {
        InitializeIfNeed();
    }

    private void InitializeIfNeed()
    {
        if (!_initialized)
        {
            _text = GetComponent<TextMeshProUGUI>();
            _defaultTextSharedMaterial = _text.fontSharedMaterial;
            _initialized = true;
        }
    }

    private void Update()
    {
        if (_displayedValue != _targetValue)
        {
            _currentValue = _displayedValue < _targetValue
                ? Math.Min(_currentValue + _changeSpeed * Time.deltaTime, _targetValue)
                : Math.Max(_currentValue - _changeSpeed * Time.deltaTime, _targetValue);

            _displayedValue = _currentValue;
            UpdateText();
        }
    }

    public TMP_Text GetTextComponent()
    {
        return _text;
    }

    public void UpdateTargetValue(double newTargetValue, bool animated)
    {
        SetTargetValue(newTargetValue, animated);
    }

    public void SetTargetValue(double targetValue, bool animated)
    {
        _completeHandled = false;
        if (animated)
        {
            if (Math.Abs(_displayedValue - targetValue) < double.Epsilon)
            {
                ApplyTargetValueImmediately(targetValue);
            }
            else
            {
                _targetValue = targetValue;
                _changeSpeed = Math.Max(_minChangeSpeed, Math.Abs(targetValue - _displayedValue) / _duration);
            }
        }
        else
        {
            ApplyTargetValueImmediately(targetValue);
        }
    }

    private void ApplyTargetValueImmediately(double targetValue)
    {
        _changeSpeed = 0d;
        _currentValue = targetValue;

        _targetValue = targetValue;
        _displayedValue = targetValue;

        UpdateText();
    }

    private void UpdateText()
    {
        InitializeIfNeed();
        var roundDisplayedValue = Math.Round(_displayedValue);
        switch (_type)
        {
            case Type.BASE:
                _text.text = roundDisplayedValue.ToString();
                break;
            case Type.FORMATTED_STRING:
                _text.text = string.Format(_formatString, roundDisplayedValue);
                break;
            case Type.CUSTOM_FORMATTER:
                _text.text = _customFormatter.Invoke(roundDisplayedValue);
                break;
            case Type.FORMATTED_STRING_WITH_CUSTOM_FORMATTER:
                _text.text = string.Format(_formatString, _customFormatter.Invoke(roundDisplayedValue));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_prevDisplayedValue != _displayedValue)
            _onValueChanged?.Invoke(_displayedValue, _displayedValue - _prevDisplayedValue);

        if (_targetValue == _displayedValue && !_completeHandled)
        {
            _completeHandled = true;
            _onComplete?.Invoke();
        }

        _prevDisplayedValue = _displayedValue;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
    }

    public void SetFormatString(string formatString)
    {
        _formatString = formatString;
        _type = Type.FORMATTED_STRING;
    }

    public void SetFormatStringKey(string formatStringKey)
    {
        _formatStringKey = formatStringKey;
        _type = Type.FORMATTED_LOCALIZED_STRING;
    }

    public void SetCustomFormatter(CustomFormatter formatter)
    {
        _customFormatter = formatter;
        _type = Type.CUSTOM_FORMATTER;
    }

    public void SetFormatStringWithCustomFormatter(string formatString, CustomFormatter formatter)
    {
        _formatString = formatString;

        _customFormatter = formatter;

        _type = Type.FORMATTED_STRING_WITH_CUSTOM_FORMATTER;
    }

    public void SetCustomFormatter(string formatStringKey, CustomFormatter formatter)
    {
        _formatStringKey = formatStringKey;
        _customFormatter = formatter;
        _type = Type.LOCALIZED_CUSTOM_FORMATTER;
    }

    public void SetCompleteHandler(CompleteHandler onComplete)
    {
        _onComplete = onComplete;
    }

    public void SetChangeHandler(ChangeHandler onValueChanged)
    {
        _onValueChanged = onValueChanged;
    }

    private enum Type
    {
        BASE,
        FORMATTED_STRING,
        FORMATTED_LOCALIZED_STRING,
        CUSTOM_FORMATTER,
        LOCALIZED_CUSTOM_FORMATTER,
        FORMATTED_STRING_WITH_CUSTOM_FORMATTER
    }

    public void SetTextMaterial(Material material)
    {
        _text.fontMaterial = material;
    }

    public void ResetTextMaterial()
    {
        _text.fontMaterial = _defaultTextSharedMaterial;
    }
}