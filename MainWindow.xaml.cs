using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TelemetryViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ushort[]> frames; // Хранит все кадры данных, загруженные из файла
        private const int ServicePartSize = 31; // Размер служебной части кадра
        private const int InformationPartSize = 512; // Размер информационной части кадра
        private const int FrameSize = ServicePartSize + InformationPartSize; // Полный размер кадра

        public MainWindow()
        {
            // Инициализация списка кадров и установка обработчиков событий для DataGrid
            InitializeComponent();
            frames = new List<ushort[]>();
            InitializeDataGrids();
            ServiceDataGrid.SelectedCellsChanged += ServiceDataGrid_SelectedCellsChanged;
            FrameDataGrid.SelectedCellsChanged += FrameDataGrid_SelectedCellsChanged;
            // Инициализация серий данных для графика и гистограммы
            InfoPartSeries = new SeriesCollection();
            var lineSeries = new LineSeries
            {
                Title = "Значение слова",
                Values = new ChartValues<double>()
            };
            InfoPartSeries.Add(lineSeries);
            DataContext = this;
            HistogramSeries = new SeriesCollection();
            HistogramChart.DataContext = this;
        }
        /// <summary>
        /// Индекс текущего отображаемого кадра
        /// </summary>
        public int CurrentFrameIndex { get; set; }
        /// <summary>
        /// Коллекция серий для отображения информационной части кадра
        /// </summary>
        public SeriesCollection InfoPartSeries { get; set; }
        /// <summary>
        /// Метки для оси X графика и гистограммы
        /// </summary>
        public List<string> Labels { get; set; }
        /// <summary>
        /// Коллекция серий для отображения гистограммы
        /// </summary>
        public SeriesCollection HistogramSeries { get; set; }
        /// <summary>
        /// Инициализация структуры таблицы данных
        /// </summary>
        private void InitializeDataGrids()
        {
            // Инициализация ServiceDataGrid с 31 столбцом
            for (int i = 0; i < ServicePartSize; i++)
            {
                ServiceDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"S{i + 1}",
                    Binding = new System.Windows.Data.Binding($"[{i}]")
                });
            }

            // Инициализация FrameDataGrid с 32 столбцами
            for (int i = 0; i < 32; i++)
            {
                FrameDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"I{i + 1}",
                    Binding = new System.Windows.Data.Binding($"[{i}]")
                });
            }
        }
        /// <summary>
        /// Инициализация графика с данными информационной части кадра.
        /// В зависимости от выбранного режима (HEX или DEC), данные преобразуются и отображаются на графике.
        /// </summary>
        /// <param name="infoPart">Информационная часть кадра в формате ushort[].</param>
        private void InitializeCharts(ushort[] infoPart)
        {
            var values = new ChartValues<string>();
            Labels = Enumerable.Range(1, 512).Select(x => x.ToString()).ToList();

            // Преобразуем данные в зависимости от выбранной системы (HEX или DEC)
            if (DecRadioButton.IsChecked == true)
            {
                foreach (var word in infoPart)
                {
                    values.Add(((word >> 1) & 0x00FF).ToString("X"));
                    InfoPartSeries[0].LabelPoint = point => $"{(ushort)point.Y}";

                }
            }
            else
            {
                foreach (var word in infoPart)
                {
                    values.Add((((word >> 1) & 0x00FF) & 0xFF).ToString("X"));
                    InfoPartSeries[0].LabelPoint = point => $"{(ushort)point.Y:X}";
                }
            }
            // Очистка и добавление новых значений в график
            InfoPartSeries[0].Values.Clear();
            foreach (var value in values)
            {
                InfoPartSeries[0].Values.Add(Convert.ToDouble(Convert.ToInt32(value, 16)));
            }
            DataContext = this;
        }
        /// <summary>
        /// Обновление гистограммы на основе данных информационной части кадра.
        /// В зависимости от выбранного режима (HEX или DEC), данные преобразуются и гистограмма перестраивается.
        /// </summary>
        /// <param name="infoPart">Информационная часть кадра в формате ushort[].</param>
        private void UpdateHistogram(ushort[] infoPart)
        {
            if (infoPart == null || infoPart.Length == 0)
                return;
            // Подготавливаем данные для гистограммы
            var values = new List<double>();
            if (DecRadioButton.IsChecked == true)
            {
                foreach (var word in infoPart)
                {
                    values.Add((word >> 1) & 0x00FF);
                }
            }
            else
            {
                foreach (var word in infoPart)
                {
                    values.Add(((word >> 1) & 0x00FF) & 0xFF);
                }
            }

            // Группируем значения и считаем их количество
            var groupedValues = values.GroupBy(v => v).OrderBy(g => g.Key).ToList();

            // Создаем ChartValues для гистограммы
            ChartValues<ObservableValue> histogramValues = new ChartValues<ObservableValue>();
            List<string> labels = new List<string>();
            List<string> hexLabels = new List<string>();
            foreach (var group in groupedValues)
            {
                histogramValues.Add(new ObservableValue(group.Count()));
                labels.Add(group.Key.ToString());
                hexLabels.Add($"{Convert.ToInt32(group.Key):X}");
            }
            // Создаем новую серию для гистограммы
            var histogramSeries = new ColumnSeries
            {
                Title = "Количество слов",
                Values = histogramValues
            };

            // Очищаем предыдущие данные и добавляем новую серию
            HistogramSeries.Clear();
            HistogramSeries.Add(histogramSeries);

            // Обновление осей гистограммы
            HistogramChart.AxisX.Clear();
            HistogramChart.AxisX.Add(new Axis
            {
                Title = "Значение",
                Labels = DecRadioButton.IsChecked == true ? labels.ToArray() : hexLabels.ToArray()
            });

            HistogramChart.AxisY.Clear();
            HistogramChart.AxisY.Add(new Axis
            {
                Title = "Частота",
                LabelFormatter = value => value.ToString("F0")
            });
        }
        /// <summary>
        /// Обработчик события нажатия кнопки открытия файла.
        /// Открывает диалоговое окно для выбора файла .kdr, загружает данные кадров из файла и отображает первый кадр.
        /// </summary>
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "KDR files (*.kdr)|*.kdr",
                Title = "Открыть файл данных"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBlock.Text = openFileDialog.FileName;
                LoadFrames(openFileDialog.FileName);
                if (frames.Any())
                {
                    FrameSlider.Maximum = frames.Count - 1;
                    FrameSlider.Value = 0; // Убедиться, что ползунок начинается с первого кадра
                    DisplayFrame(0); // Отображение первого кадра
                }
            }
        }
        /// <summary>
        /// Загрузка данных из файла формата .kdr
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadFrames(string filePath)
        {
            frames.Clear(); // Очистка списка кадров
            var content = File.ReadAllText(filePath);

            // Разбиваем содержимое на части, используя "=KADR=" в качестве разделителя
            var framesData = content.Split(new[] { "=KADR=     " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var frameData in framesData)
            {
                // Убираем лишние пробелы и разбиваем строку на отдельные элементы
                var frameElements = frameData.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (frameElements.Length == FrameSize)
                {
                    var frame = new ushort[FrameSize];
                    // Преобразование каждого элемента кадра из шестнадцатеричного формата в ushort
                    for (int i = 0; i < FrameSize; i++)
                    {
                        frame[i] = Convert.ToUInt16(frameElements[i], 16);
                    }
                    frames.Add(frame); // Добавление сформированного кадра в список
                }
                else
                {
                    // Обработка ошибки: количество элементов не совпадает с FrameSize
                    MessageBox.Show($"Ожидалось {FrameSize} элементов, но получено {frameElements.Length}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (frames.Count == 0)
            {
                MessageBox.Show("Не удалось найти ни одного кадра в файле.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Обработчик изменения значения слайдера кадров
        /// </summary>
        private void FrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FrameSlider.Value % 1 == 0) // Убеждаемся, что значение слайдера целочисленное
            {
                int frameIndex = (int)e.NewValue;
                CurrentFrameIndex = frameIndex;
                DisplayFrame(frameIndex); // Отображение выбранного кадра
            }
        }
        /// <summary>
        /// Отображение информации о текущем кадре
        /// </summary>
        /// <param name="frameIndex">Индекс кадра</param>
        private void DisplayFrame(int frameIndex)
        {
            // Проверяем, что список frames не равен null и что frameIndex находится в допустимом диапазоне
            if (frames != null && frameIndex >= 0 && frameIndex < frames.Count)
            {
                var frame = frames[frameIndex];
                CurrentFrameTextBlock.Text = $"Кадр {frameIndex + 1}";

                var servicePart = frame.Take(ServicePartSize).Select(x => (x & 0x03FF).ToString("X")).ToArray();
                var infoPart = frame.Skip(ServicePartSize).Select(x => ((x >> 1) & 0x00FF).ToString("X")).ToArray();

                // Если выбран режим DEC (десятичная система), преобразуем данные
                if (DecRadioButton.IsChecked == true)
                {
                    servicePart = frame.Take(ServicePartSize).Select(x => ((x & 0x03FF) & 0x1FF).ToString()).ToArray();
                    infoPart = frame.Skip(ServicePartSize).Select(x => (((x >> 1) & 0x00FF) & 0xFF).ToString()).ToArray();
                }

                // Отображаем служебную часть (одна строка, 31 столбец)
                ServiceDataGrid.ItemsSource = new List<string[]> { servicePart };

                // Отображаем информационную часть (32 столбца, 16 строк)
                var infoPartGrid = new List<string[]>();
                for (int i = 0; i < 16; i++)
                {
                    infoPartGrid.Add(infoPart.Skip(i * 32).Take(32).ToArray());
                }
                FrameDataGrid.ItemsSource = infoPartGrid;
                // Инициализация и обновление графика и гистограммы для информационной части кадра
                InitializeCharts(frame.Skip(ServicePartSize).ToArray());
                UpdateHistogram(frame.Skip(ServicePartSize).ToArray());
            }
        }
        /// <summary>
        /// Обработчик изменения выбранных ячеек в таблице служебной части
        /// </summary>
        private void ServiceDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // Получаем выбранную ячейку
            DataGridCellInfo cellInfo = e.AddedCells.FirstOrDefault();
            if (cellInfo != null && cellInfo.Column != null)
            {
                // Получение индекса выбранного столбца и отображение информации о нем
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid != null)
                {
                    DataGridCell cell = GetCell(dataGrid, cellInfo);
                    if (cell != null)
                    {
                        int columnIndex = cellInfo.Column.DisplayIndex + 1; // DisplayIndex начинается с 0
                        CurrentPositionTextBlock.Text = $"Служебная часть: слово {columnIndex}";
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик изменения выбранных ячеек в таблице информационной части
        /// </summary>
        private void FrameDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridCellInfo cellInfo = e.AddedCells.FirstOrDefault();
            if (cellInfo != null && cellInfo.Column != null)
            {
                // Получаем строку и столбец
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid != null)
                {
                    DataGridCell cell = GetCell(dataGrid, cellInfo);
                    if (cell != null)
                    {
                        int rowIndex = dataGrid.Items.IndexOf(cellInfo.Item) + 1; // Получаем индекс строки
                        int columnIndex = cellInfo.Column.DisplayIndex + 1; // DisplayIndex начинается с 0
                        int wordPosition = (rowIndex - 1) * 32 + columnIndex; // Вычисляем позицию слова
                        CurrentPositionTextBlock.Text = $"Информационная часть: слово {wordPosition}";
                    }
                }
            }
        }
        /// <summary>
        /// Получение ячейки DataGrid по информации о ней
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="cellInfo"></param>
        /// <returns></returns>
        private DataGridCell GetCell(DataGrid dataGrid, DataGridCellInfo cellInfo)
        {
            if (cellInfo != null && cellInfo.Item != null && cellInfo.Column != null)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
                if (row != null)
                {
                    int columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                    if (columnIndex > -1)
                    {
                        DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                        if (presenter != null)
                        {
                            return presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                        }
                    }
                }
            }
            return null;
        }
        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        /// <summary>
        /// Изменение режима (HEX или DEC), вызывает перерисовку данных
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (HexRadioButton.IsChecked == true)
            {
                DisplayFrame(CurrentFrameIndex);
            }
            else if (DecRadioButton.IsChecked == true)
            {
                DisplayFrame(CurrentFrameIndex);
            }
        }
    }
}
