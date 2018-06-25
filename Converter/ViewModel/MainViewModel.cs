using Converter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Converter.ViewModel
{
    class MainViewModel : NotifyPropertyChangedObject
    {
        private Repository repository = new Repository();
        private IView view;
        private IEnumerable<Currency> currencyList;
        public IEnumerable<Currency> CurrencyList
        {
            get
            {
                return this.currencyList;
            }
            set
            {
                this.currencyList = value;
                OnPropertyChanged();
            }
        }

        private Currency comboBoxFromSelectedItem;
        private Currency ComboBoxFromSelectedItem
        {
            get
            {
                return comboBoxFromSelectedItem;
            }
            set
            {
                comboBoxFromSelectedItem = value;
                OnPropertyChanged();
            }
        }

        private Currency comboBoxToSelectedItem;
        private Currency ComboBoxToSelectedItem
        {
            get
            {
                return comboBoxToSelectedItem;
            }
            set
            {
                comboBoxToSelectedItem = value;
                OnPropertyChanged();
            }
        }

        private double inputAmount;
        public double InputAmount
        {
            get
            {
                return inputAmount;
            }
            set
            {
                inputAmount = value;
                OnPropertyChanged();
            }
        }

        private double outputAmount;
        public double OutputAmount
        {
            get
            {
                return outputAmount;
            }
            set
            {
                outputAmount = value;
                OnPropertyChanged();
            }
        }

        private ICommand selectionChangedFromCommand;
        public ICommand SelectionChangedFromCommand
        {
            get
            {
                if (this.selectionChangedFromCommand is null)
                {
                    this.selectionChangedFromCommand = new RelayCommand(
                       async (param) =>
                        {
                            if(param as Currency != null)
                            {
                                comboBoxFromSelectedItem = param as Currency;
                                try
                                {
                                    await repository.GetFileAsync(comboBoxFromSelectedItem);
                                }
                                catch (Exception ex)
                                {
                                    view.Alert(ex.Message);
                                }
                                
                            }
                        }
                    );
                }
                return this.selectionChangedFromCommand;
            }
        }

        private ICommand selectionChangedToCommand;
        public ICommand SelectionChangedToCommand
        {
            get
            {
                if (this.selectionChangedToCommand is null)
                {
                    this.selectionChangedToCommand = new RelayCommand(
                        (param) =>
                        {
                            if (param as Currency != null)
                            {
                                comboBoxToSelectedItem = param as Currency;
                            }
                        }
                    );
                }
                return this.selectionChangedToCommand;
            }
        }

        private ICommand convertCommand;
        public ICommand ConvertCommand
        {
            get
            {
                if (this.convertCommand is null)
                {
                    this.convertCommand = new RelayCommand(
                       async (param) =>
                        {
                            try
                            {
                                if(ComboBoxFromSelectedItem == ComboBoxToSelectedItem)
                                {
                                    OutputAmount = InputAmount;
                                }
                                else
                                {
                                    double rate = await repository.GetRateAsync(ComboBoxFromSelectedItem, ComboBoxToSelectedItem);
                                    OutputAmount = InputAmount * rate;
                                }
                                
                            }
                            catch(Exception ex)
                            {
                                view.Alert(ex.Message);
                            }
                        },
                        (param) =>
                        {
                            return (ComboBoxFromSelectedItem != null) && (ComboBoxToSelectedItem != null) && (InputAmount > 0);
                        }
                    );
                }
                return this.convertCommand;
            }
        }

        public MainViewModel(IView view)
        {
            this.view = view;
            try
            {
                this.CurrencyList = this.repository.GetAllCurrency("Source.html");
            }
            catch (Exception ex)
            {
                view.Alert(ex.Message);
            }
        }
    }
}
