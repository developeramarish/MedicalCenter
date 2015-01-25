﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MedicalCenter.Services;

namespace MedicalCenter.GUI.Registrar
{
    /// <summary>
    /// Obsługa zdarzeń użytkownika dla widoku szczegółów pacjenta (działania na modelach i serwisach pod wpływem zdarzeń z widoku).
    /// </summary>
    public class PatientDetailsPresenter
    {
        #region Private fields

        /// <summary>
        /// Logika biznesowa w zakresie pacjentów.
        /// </summary>
        PatientBusinessService patientBusinessService;

        /// <summary>
        /// Widok szczegółów pacjenta zarządzany przez tego prezentera.
        /// </summary>
        PatientDetailsView view;

        /// <summary>
        /// Określa czy podczas walidacji pól formularza znalezione zostały błędy.
        /// </summary>
        bool validationErrors;

        /// <summary>
        /// Instancja struktury Thickness, definiująca grubość linii o wartości 1.0, używana przy obramowaniach elementów formularza.
        /// </summary>
        System.Windows.Thickness thickness1;

        /// <summary>
        /// Instancja struktury Thickness, definiująca grubość linii o wartości 2.0, używana przy obramowaniach elementów formularza.
        /// </summary>
        System.Windows.Thickness thickness2;

        /// <summary>
        /// Minimalna data dla numeru PESEL.
        /// </summary>
        DateTime minDate;

        /// <summary>
        /// Maksymalna data dla numeru PESEL.
        /// </summary>
        DateTime maxDate;

        #endregion // Private fields

        #region Private properties

        /// <summary>
        /// Określa czy formularz został poprawnie wypełniony.
        /// </summary>
        bool IsFormCompleted
        {
            get
            {
                if (!validationErrors &&
                    view.LastName.Text.Length > 0 &&
                    view.FirstName.Text.Length > 0 &&
                    view.BirthDate.SelectedDate != null &&
                    view.Pesel.Text.Length > 0 &&
                    view.BuildingNumber.Text.Length > 0 &&
                    view.PostalCode.Text.Length > 0 &&
                    view.City.Text.Length > 0)

                    return true;

                else
                    return false;
            }
        }

        #endregion // Private properties

        #region Ctors

        /// <summary>
        /// Konstruktor zapisujący referencję do zarządzanego widoku.
        /// </summary>
        /// <param name="view">Widok menu głównego rejestratorki zarządzany przez tego prezentera.</param>
        public PatientDetailsPresenter(PatientDetailsView view)
        {
            this.view = view;
            this.patientBusinessService = new PatientBusinessService();
            this.validationErrors = false;
            this.thickness1 = new System.Windows.Thickness(1.0);
            this.thickness2 = new System.Windows.Thickness(2.0);
            this.minDate = new DateTime(1800, 1, 1);
            this.maxDate = new DateTime(2299, 12, 31);
        }

        #endregion // Ctors

        #region Public methods

        #region Generic methods

        /// <summary>
        /// Sprawdza, czy wskazany klawisz należy do wskazanej grupy klawiszy.
        /// Dostępne grupy: cyfry, litery lub spacja, obie grupy.
        /// Niezależnie od wybranej grupy, zawsze sprawdzane jest, czy wskazany klawisz to delete, backspace, tab, return, escape lub enter.
        /// </summary>
        /// <param name="key">Klawisz do sprawdzenia.</param>
        /// <param name="kindOfKey">
        /// Rodzaj klawisza do sprawdzenia:
        /// NUM - cyfra (również z klaw. numerycznej)
        /// LET - litera lub spacja (sprawdza również alt i shift)
        /// LAN - litera lub spacja lub cyfra
        /// </param>
        /// <returns>true jeśli jest to jeden z tych klawiszy, w przeciwnym razie false</returns>
        /// <exception cref="ArgumentNullException">Drugi argument ma wartość null.</exception>
        public bool KindOfKey(Key key, string kindOfKey)
        {
            // rzucenie wyjątkiem, jeśli drugi argument jest null'em
            if (kindOfKey == null)
                throw new ArgumentNullException("kindOfKey", "Nazwa grupy klawiszy ma wartość null");
            
            bool retval = false;
            
            // sprawdzenie czy klawisz jest cyfrą
            if (!kindOfKey.Equals("LET") &&
                ((key >= Key.D0 && key <= Key.D9 && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) ||
                 (key >= Key.NumPad0 && key <= Key.NumPad9)))
            {
                retval = true;
            }
            // sprawdzenie czy klawisz jest literą lub spacją (+ ew. wciśnięty alt i/lub shift)
            if (!kindOfKey.Equals("NUM") &&
                ((key >= Key.A && key <= Key.Z) ||
                  key == Key.Space ||
                 Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)))
            {
                retval = true;
            }
            // sprawdzenie czy klawisz jest jednym z wymienionych klawiszy specjalnych
            if (key == Key.Delete || key == Key.Back || key == Key.Tab || key == Key.Return || key == Key.Escape || key == Key.Enter)
                retval = true;

            return retval;
        }

        /// <summary>
        /// Usuwa z pola tekstowego wszystkie znaki nienależące do wskazanej grupy znaków.
        /// Obsługiwane grupy znaków: cyfry, litery i spacja, obie grupy naraz.
        /// </summary>
        /// <param name="textBox">Pole tekstowe, z którego mają zostać usunięte nieprawidłowe znaki.</param>
        /// <param name="charactersGroup">
        /// Oznaczenie grupy prawidłowych znaków:
        /// NUM - cyfry
        /// LET - litery i spacja
        /// LAN - cyfry, litery i spacja
        /// </param>
        /// <exception cref="ArgumentNullException">Jeden z argumentów ma wartość null.</exception>
        public void TextBoxChanged(TextBox textBox, string charactersGroup)
        {
            bool change = false;
            
            // rzucenie wyjątkiem, jeśli któryś z argumentów jest null'em
            if (textBox == null)
                throw new ArgumentNullException("textBox", "Pole tekstowe ma wartość null");
            if (charactersGroup == null)
                throw new ArgumentNullException("charactersGroup", "Nazwa grupy znaków ma wartość null");

            // jeśli wybrana grupa to cyfry + ew. litery i spacja
            if (!charactersGroup.Equals("LET"))
            {
                bool both = (charactersGroup.Equals("LAN")) ? true : false;

                foreach (char c in textBox.Text)
                {
                    // jeśli to nie cyfra            (i ew.           nie litera i nie spacja)
                    if (!char.IsDigit(c) && (!both || (!char.IsLetter(c) && c != ' ')))
                    {
                        // znajdź i zamień
                        textBox.Text = textBox.Text.Replace(c.ToString(), "");

                        change = true;
                    }
                }
            }
            // jeśli wybrana grupa to litery i spacja
            else
            {
                foreach (char c in textBox.Text)
                {
                    if (!char.IsLetter(c) && c != ' ')
                    {
                        textBox.Text = textBox.Text.Replace(c.ToString(), "");

                        change = true;
                    }
                }
            }

            if (change)
                textBox.CaretIndex = textBox.Text.Length;
        }

        /// <summary>
        /// Obsługa zdarzenia utraty focus'a klawiatury przez pole tekstowe w widoku szczegółów pacjenta.
        /// </summary>
        public void TextBoxLostFocus()
        {
            // jeśli formularz został poprawnie wypełniony, należy aktywować przycisk "Zapisz"
            view.Save.IsEnabled = IsFormCompleted;
        }

        #endregion // Generic methods

        #region Detailed methods

        /// <summary>
        /// Obsługa zdarzenia kliknięcia przycisku "Powrót" w widoku szczegółów pacjenta.
        /// </summary>
        public void Back(bool question)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.DialogResult.Yes;

            // jeśli istnieje potrzeba, użytkownik pytany jest o potwierdzenie chęci powrotu do menu głównego bez zapisywania wprowadzonych zmian;
            // tylko jeśli użytkownik kliknął przycisk "Tak", należy wyczyścić wszystkie pola i wrócić do menu głównego
            if (question)
                dialogResult = System.Windows.Forms.MessageBox.Show("Czy na pewno chcesz wrócić do menu głównego? Wszelkie zmiany nie zostaną zapisane.",
                                                     "Pytanie",
                                                     System.Windows.Forms.MessageBoxButtons.YesNo,
                                                     System.Windows.Forms.MessageBoxIcon.Question,
                                                     System.Windows.Forms.MessageBoxDefaultButton.Button2);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                // ustawienie wartości domyślnych w widoku szczegółów pacjenta
                view.LastName.Clear();
                view.FirstName.Clear();
                view.SecondName.Clear();
                view.BirthDate.SelectedDate = null;
                view.Gender.SelectedIndex = 0;
                view.Pesel.Clear();
                view.Street.Clear();
                view.BuildingNumber.Clear();
                view.Apartment.Clear();
                view.PostalCode.Clear();
                view.City.Clear();
                view.Post.Clear();
                view.IsInsured.IsChecked = true;

                // powrót do menu głównego
                view.ParentWindow.ContentArea.Content = view.ParentWindow.RegistrarMainMenuView;
            }
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia przycisku "Zapisz" w widoku szczegółów pacjenta.
        /// </summary>
        public void SaveChanges()
        {
            long pesel;

            // próba konwersji PESEL'u ze string'a na 8-bajtowy int
            if (long.TryParse(view.Pesel.Text, out pesel))
            {
                // jeśli konwersja się powiodła, należy zapisać PESEL w postaci liczbowej do obiektu z danymi pacjenta
                view.PatientData.Pesel = pesel;
                
                // zapis informacji o pacjencie do bazy danych
                bool? saved = patientBusinessService.SavePatient(view.PatientData);

                // PESEL nie jest unikatowy - błąd
                if (saved == null)
                    System.Windows.Forms.MessageBox.Show("Podany numer PESEL już istnieje w bazie danych!",
                                                         "Nieprawidłowy PESEL!",
                                                         System.Windows.Forms.MessageBoxButtons.OK,
                                                         System.Windows.Forms.MessageBoxIcon.Warning);
                // wystąpił inny błąd
                else if (saved == false)
                    System.Windows.Forms.MessageBox.Show("Wystąpił błąd podczas próby zapisu informacji o pacjencie w bazie danych.",
                                                         "Błąd zapisu!",
                                                         System.Windows.Forms.MessageBoxButtons.OK,
                                                         System.Windows.Forms.MessageBoxIcon.Error);
                // prawidłowy zapis
                else
                {
                    System.Windows.Forms.MessageBox.Show("Informacje o pacjencie zostały pomyślnie zapisane w bazie danych.",
                                                         "Pomyślny zapis",
                                                         System.Windows.Forms.MessageBoxButtons.OK,
                                                         System.Windows.Forms.MessageBoxIcon.Information);

                    Back(false);
                }
            }
            // jeśli próba konwersji PESEL'u się nie powiodła, nie można zapisać danych pacjenta do bazy
            else
            {
                System.Console.WriteLine("Błąd podczas konwersji numeru PESEL z postaci stringowej na liczbę");

                System.Windows.Forms.MessageBox.Show("Wystąpił nieznany błąd.",
                                                     "Nieznany błąd!",
                                                     System.Windows.Forms.MessageBoxButtons.OK,
                                                     System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zmiany zawartości pola tekstowego "Pesel" w widoku szczegółów pacjenta.
        /// </summary>
        public void PeselChanged()
        {
            // jeśli pole nie jest edytowane, to wprowadzona wartość pochodzi ze zbindowanego obiektu "Patient PatientData"
            if (!view.Pesel.IsKeyboardFocused)
            {
                // pesel to liczba - jeśli jest zerem, to pole na pesel ma być puste
                if (view.Pesel.Text == "0")
                    view.Pesel.Clear();
                // pesel to liczba - brak zer z lewej strony
                else if (view.Pesel.Text.Length == 10)
                    view.Pesel.Text = "0" + view.Pesel.Text;
            }

            // usunięcie z pola nieprawidłowych znaków
            TextBoxChanged(view.Pesel, "NUM");
        }

        /// <summary>
        /// Walidacja wartości wprowadzonej do pola "Nr PESEL" dla pól powiązanych: "Nr PESEL", "Data urodzenia", "Płeć".
        /// </summary>
        public void ValidatePesel()
        {
            // musi być co walidować - pole na pesel nie może być puste
            if (view.Pesel.Text.Length > 0)
            {
                // określa czy podany pesel jest nieprawidłowy
                byte incorrectPesel = 0;

                // jeśli mniej niż 11 znaków, pesel jest nieprawidłowy
                if (view.Pesel.Text.Length < 11)
                {
                    incorrectPesel = 1;
                    view.Pesel.ToolTip = "Pesel musi mieć dokładnie 11 znaków!";
                }
                else
                {
                    int temp;

                    // jeśli podano datę urodzenia, można zwalidować pod tym kątem pesel
                    if (view.BirthDate.SelectedDate != null)
                    {
                        // dwie pierwsze cyfry pesel'u to młodsza połowa roku
                        int.TryParse(view.Pesel.Text.Substring(0, 2), out temp);

                        // jeśli młodsza połowa numeru roku w podanej dacie urodzenia jest inna niż ta podana w peselu,
                        // to należy zgłosić błąd w tych dwóch powiązanych ze sobą polach
                        if (view.BirthDate.SelectedDate.Value.Year % 100 != temp)
                            incorrectPesel = 2;
                        else
                        {
                            // trzecia i czwarta cyfra pesel'u tworzą nr miesiąca
                            int.TryParse(view.Pesel.Text.Substring(2, 2), out temp);

                            // miesiąc z daty urodzenia inny niż podany w pesel'u -> błąd
                            // (szczegóły tutaj: https://pl.wikipedia.org/wiki/PESEL#Data_urodzenia)
                            if (view.BirthDate.SelectedDate.Value.Year >= 1900 && view.BirthDate.SelectedDate.Value.Year < 2000)
                            {
                                if (view.BirthDate.SelectedDate.Value.Month != temp)
                                    incorrectPesel = 2;
                            }
                            else if (view.BirthDate.SelectedDate.Value.Year >= 2000 && view.BirthDate.SelectedDate.Value.Year < 2100)
                            {
                                if (view.BirthDate.SelectedDate.Value.Month != temp - 20)
                                    incorrectPesel = 2;
                            }
                            else if (view.BirthDate.SelectedDate.Value.Year >= 2100 && view.BirthDate.SelectedDate.Value.Year < 2200)
                            {
                                if (view.BirthDate.SelectedDate.Value.Month != temp - 40)
                                    incorrectPesel = 2;
                            }
                            else if (view.BirthDate.SelectedDate.Value.Year >= 2200 && view.BirthDate.SelectedDate.Value.Year < 2300)
                            {
                                if (view.BirthDate.SelectedDate.Value.Month != temp - 60)
                                    incorrectPesel = 2;
                            }
                            else if (view.BirthDate.SelectedDate.Value.Year >= 1800 && view.BirthDate.SelectedDate.Value.Year < 1900)
                            {
                                if (view.BirthDate.SelectedDate.Value.Month != temp - 80)
                                    incorrectPesel = 2;
                            }
                            else
                                incorrectPesel = 2;

                            if(incorrectPesel == 0)
                            {
                                // piąta i szósta cyfra pesel'u tworzą dzień miesiąca
                                int.TryParse(view.Pesel.Text.Substring(4, 2), out temp);

                                // dzień z daty urodzenia inny niż podany w pesel'u -> błąd
                                if (view.BirthDate.SelectedDate.Value.Day != temp)
                                    incorrectPesel = 2;
                            }
                        }
                    }

                    // walidacja pesel'u pod kątem płci
                    int.TryParse(view.Pesel.Text.Substring(9, 1), out temp);

                    // cyfra płci nieparzysta == mężczyzna, parzysta == kobieta
                    // na liście płci jest odwrotnie: SelectedIndex == 0 -> mężczyzna
                    // więc jeśli w pesel'u cyfra jest nieparzysta i SelectedIndex listy płci też jest nieparzysty (1) => błąd
                    if ((temp & 1) == view.Gender.SelectedIndex)
                    {
                        // jeśli wcześniej wykryto błąd na linii pesel <-> data urodzenia
                        if (incorrectPesel == 2)
                            incorrectPesel = 4;
                        else
                            incorrectPesel = 3;
                    }

                    // sprawdzanie cyfry kontrolnej
                    double[] multipliers = { 1.0, 3.0, 7.0, 9.0, 1.0, 3.0, 7.0, 9.0, 1.0, 3.0, 1.0, 0.0 };

                    // walidacja cyfry kontrolnej sprowadza się do następującego działania:
                    // obliczenia wyrażenia a+3b+7c+9d+e+3f+7g+9h+i+3j+k (gdzie litery oznaczają kolejne cyfry numeru)
                    for (int i = 0; i < view.Pesel.Text.Length; ++i)
                        multipliers[11] += char.GetNumericValue(view.Pesel.Text[i]) * multipliers[i];

                    // a następnie sprawdzenia czy reszta z dzielenia przez 10 jest zerem
                    if ((int)multipliers[11] % 10 != 0)
                        incorrectPesel += 10;
                }

                // jeśli pesel jest nieprawidłowy, należy to zaznaczyć w formularzu
                if (incorrectPesel > 0)
                {
                    // ustawienie flagi błędów w formularzu
                    validationErrors = true;

                    // ustawienie czerwonych obramowań odpowiednim polom
                    view.Pesel.BorderBrush = System.Windows.Media.Brushes.Red;
                    view.Pesel.BorderThickness = thickness2;

                    // komunikat o nieprawidłowej sumie kontrolnej numeru PESEL
                    string wrongChecksum = string.Empty;

                    // zgłoszenie tego faktu odbywa się poprzez dodanie do incorrectPesel wartości 10
                    if (incorrectPesel > 9)
                    {
                        wrongChecksum = "Suma kontrolna numeru PESEL nie zgadza się!";
                        incorrectPesel -= 10;
                    }

                    if (incorrectPesel == 0 && wrongChecksum.Length > 0)
                    {
                        view.Pesel.ToolTip = wrongChecksum;

                        view.BirthDate.BorderBrush = view.Gender.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
                        view.BirthDate.BorderThickness = view.Gender.BorderThickness = thickness1;
                        view.BirthDate.ToolTip = view.Gender.ToolTip = null;
                    }
                    else
                    {
                        if (wrongChecksum.Length > 0)
                            wrongChecksum = "\n" + wrongChecksum;

                        // jeśli błąd dotyczy także wartości dwóch pozostałych pól
                        if (incorrectPesel == 4)
                        {
                            view.BirthDate.BorderBrush = view.Gender.BorderBrush = System.Windows.Media.Brushes.Red;
                            view.BirthDate.BorderThickness = view.Gender.BorderThickness = thickness2;
                            view.BirthDate.ToolTip = view.Gender.ToolTip = view.Pesel.ToolTip = "Błąd w powiązanych polach Nr PESEL, Data urodzenia, Płeć!" + wrongChecksum;
                        }
                        // jeśli błąd dotyczy także wybranej płci
                        else if (incorrectPesel == 3)
                        {
                            view.Gender.BorderBrush = System.Windows.Media.Brushes.Red;
                            view.Gender.BorderThickness = thickness2;
                            view.Gender.ToolTip = view.Pesel.ToolTip = "Numer PESEL i płeć nie zgadzają się!" + wrongChecksum;

                            view.BirthDate.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
                            view.BirthDate.BorderThickness = thickness1;
                            view.BirthDate.ToolTip = null;
                        }
                        // jeśli błąd dotyczy także wybranej daty urodzenia
                        else if (incorrectPesel == 2)
                        {
                            view.BirthDate.BorderBrush = System.Windows.Media.Brushes.Red;
                            view.BirthDate.BorderThickness = thickness2;
                            view.BirthDate.ToolTip = view.Pesel.ToolTip = "Numer PESEL i data urodzenia nie zgadzają się!" + wrongChecksum;

                            view.Gender.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
                            view.Gender.BorderThickness = thickness1;
                            view.Gender.ToolTip = null;
                        }
                        else
                        {
                            view.BirthDate.BorderBrush = view.Gender.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
                            view.BirthDate.BorderThickness = view.Gender.BorderThickness = thickness1;
                            view.BirthDate.ToolTip = view.Gender.ToolTip = null;
                        }
                    }
                }
                // pesel prawidłowy - przywracanie domyślnych ustawień pól formularza
                else
                {
                    view.Pesel.BorderThickness = view.BirthDate.BorderThickness = view.Gender.BorderThickness = thickness1;
                    view.Pesel.BorderBrush = view.BirthDate.BorderBrush = view.Gender.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
                    view.BirthDate.ToolTip = view.Gender.ToolTip = view.Pesel.ToolTip = null;

                    validationErrors = false;
                }
            }

            // jeśli formularz został poprawnie wypełniony, należy aktywować przycisk "Zapisz"
            view.Save.IsEnabled = IsFormCompleted;
        }

        /// <summary>
        /// Obsługa zdarzenia zmiany daty w polu na datę urodzenia w widoku szczegółów pacjenta.
        /// </summary>
        public void BirthDateChanged()
        {
            // jeśli wybrana jest jakaś data (selectedDate != null)
            if (view.BirthDate.SelectedDate.HasValue)
            {
                // jeśli wybrana data jest wcześniejsza niż minimalna możliwa do zweryfikowania z numerem pesel,
                // należy zmienić ją na ową minimalną dla pesel'u datę
                if (view.BirthDate.SelectedDate.Value < minDate)
                    view.BirthDate.SelectedDate = minDate;
                // analogicznie dla daty maksymalnej
                else if (view.BirthDate.SelectedDate.Value > maxDate)
                    view.BirthDate.SelectedDate = maxDate;
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zmiany zawartości pola tekstowego na miejscowość w widoku szczegółów pacjenta.
        /// </summary>
        public void CityChanged()
        {
            bool change = false;
            
            // usuń z pola wszystkie znaki niebędące cyframi, literami, spacjami ani myślnikami
            foreach (char c in view.City.Text)
            {
                if (!char.IsDigit(c) && !char.IsLetter(c) && c != ' ' && c != '-')
                {
                    // znajdź i zamień
                    view.City.Text = view.City.Text.Replace(c.ToString(), "");

                    change = true;
                }
            }

            if (change)
                view.City.CaretIndex = view.City.Text.Length;
        }

        /// <summary>
        /// Obsługa zdarzenia wciśnięcia klawisza podczas edycji pola tekstowego na miejscowość w widoku szczegółów pacjenta.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CityKeyDown(Key key)
        {
            // sprawdzenie, czy wskazany przycisk jest literą, spacją lub cyfrą
            bool retval = KindOfKey(key, "LAN");

            // jeśli żadne z powyższych:
            if (!retval)
            {
                // sprawdzenie czy klawisz ten jest myślnikiem (minusem)
                if (key == Key.Subtract || key == Key.OemMinus)
                    retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Obsługa zdarzenia zmiany zawartości pola tekstowego na kod pocztowy w widoku szczegółów pacjenta.
        /// </summary>
        public void PostalCodeChanged()
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(view.PostalCode.Text, @"^\d{2}-\d{1,3}$"))
            {
                TextBoxChanged(view.PostalCode, "NUM");

                // cyfr w polu może być maksymalnie 5
                if (view.PostalCode.Text.Length == 6)
                    view.PostalCode.Text = view.PostalCode.Text.Substring(0, 5);

                // jeśli w polu wpisanych jest więcej niż 2 cyfry, należy zadbać o obecność myślnika
                if (view.PostalCode.Text.Length > 2)
                {
                    // sprawdzenie obecności myślnika na trzeciej pozycji
                    if (view.PostalCode.Text[2] != '-')
                    {
                        // zmiana tekstu wg. szablonu ^\d{2}-\d{1,3}$
                        view.PostalCode.Text = view.PostalCode.Text.Substring(0, 2) + "-" + view.PostalCode.Text.Substring(2, view.PostalCode.Text.Length - 2);
                        
                        // ustawienie karetki w polu na końcu tekstu
                        view.PostalCode.CaretIndex = view.PostalCode.Text.Length;
                    }
                }
            }
        }

        #endregion // Detailed methods

        #endregion // Public methods
    }
}
