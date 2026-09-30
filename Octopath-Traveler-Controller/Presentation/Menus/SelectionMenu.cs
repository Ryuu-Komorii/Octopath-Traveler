using Octopath_Traveler_View;

namespace Octopath_Traveler;

public abstract class SelectionMenu<T>
    where T : class
{
    private const int FirstOptionNumber = 1;

    protected View View { get; }

    protected SelectionMenu(View view)
    {
        View = view;
    }

    protected T? SelectOption(
        string header,
        IReadOnlyList<T> options)
    {
        WriteMenu(header, options);

        int selectedOption =
            ReadSelectedOption();

        if (IsCancelOption(
            selectedOption,
            options.Count))
        {
            return null;
        }

        return options[
            GetOptionIndex(
                selectedOption)];
    }

    protected abstract string FormatOption(
        T option);

    private void WriteMenu(
        string header,
        IReadOnlyList<T> options)
    {
        View.WriteLine(
            CombatText.SeparatorLine);

        View.WriteLine(
            header);

        WriteOptions(
            options);

        WriteCancelOption(
            options.Count);
    }

    private void WriteOptions(
        IReadOnlyList<T> options)
    {
        int optionNumber =
            FirstOptionNumber;

        foreach (T option in options)
        {
            WriteOption(
                optionNumber,
                option);

            optionNumber++;
        }
    }

    private void WriteOption(
        int optionNumber,
        T option)
    {
        View.WriteLine(
            string.Format(
                CombatText.OptionFormat,
                optionNumber,
                FormatOption(option)));
    }

    private void WriteCancelOption(
        int optionCount)
    {
        int cancelOption =
            GetCancelOptionNumber(
                optionCount);

        View.WriteLine(
            string.Format(
                CombatText.OptionFormat,
                cancelOption,
                CombatText.CancelOption));
    }

    private int ReadSelectedOption()
    {
        return int.Parse(
            View.ReadLine());
    }

    private bool IsCancelOption(
        int selectedOption,
        int optionCount)
    {
        return selectedOption ==
               GetCancelOptionNumber(
                   optionCount);
    }

    private int GetOptionIndex(
        int selectedOption)
    {
        return selectedOption -
               FirstOptionNumber;
    }

    private int GetCancelOptionNumber(
        int optionCount)
    {
        return optionCount +
               FirstOptionNumber;
    }
}