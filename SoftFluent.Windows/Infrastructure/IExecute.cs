namespace Abstractions;

public interface IExecute
{
   void Executed(object sender, EventArgs executedRoutedEventArgs);
   void CanExecute(object sender, EventArgs canExecuteRoutedEventArgs);
}