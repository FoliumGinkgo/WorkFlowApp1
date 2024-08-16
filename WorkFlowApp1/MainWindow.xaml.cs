using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WorkFlowApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CustomWorkflowDesignerView designer;

        public MainWindow()
        {
            InitializeComponent();
            (new DesignerMetadata()).Register();

        }

        private void LoadWorkflow(string filePath)
        {
            WorkFlowView.Content = null;
            ToolView.Content = null;
            
            designer = new CustomWorkflowDesignerView();
            try
            {
               
                designer.Context.Services.GetService<DesignerConfigurationService>().PanModeEnabled=true;
                designer.Load(filePath);
                
                UndoEngine undoEngine = designer.Context.Services.GetService<UndoEngine>();
                DesignerView designerView = designer.Context.Services.GetService<DesignerView>();
                ModelService modelService = designer.Context.Services.GetService<ModelService>();
                ModelItem rootModelItem = modelService.Root;
                
                undoEngine.UndoUnitAdded += (ss, ee) =>
                {
                    designer.Flush();
                    Console.WriteLine($"{DateTime.Now} -> {ee.UndoUnit.Description}");
                };
                
                designerView.WorkflowShellBarItemVisibility = ShellBarItemVisibility.Arguments
                                                              | ShellBarItemVisibility.Imports
                                                              | ShellBarItemVisibility.MiniMap
                                                              | ShellBarItemVisibility.Variables
                                                              | ShellBarItemVisibility.Zoom
                                                              | ShellBarItemVisibility.PanMode;

                var view = designer.View as Grid;
                ApplyStylesUsingLogicalTree(view);
                
                WorkFlowView.Content = view;
                ToolView.Content = designer.OutlineView;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ApplyStylesUsingLogicalTree(DependencyObject parent)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is DataGrid dataGrid)
                {
                    Console.WriteLine("匹配类型：DataGrid");
                    Style cellStyle = new Style(typeof(DataGridCell));
                    cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.LightYellow));
                    cellStyle.Setters.Add(new Setter(DataGridCell.ForegroundProperty, Brushes.DarkRed));
                    cellStyle.Setters.Add(new Setter(DataGridCell.FontWeightProperty, FontWeights.Bold));
                    cellStyle.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.DarkBlue));
                    cellStyle.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
                    dataGrid.Resources[typeof(DataGridCell)] = cellStyle;
                    // 修改选中行的样式
                    Style rowStyle = new Style(typeof(DataGridRow));
                    rowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, Brushes.LightBlue));
                    rowStyle.Setters.Add(new Setter(DataGridRow.ForegroundProperty, Brushes.DarkBlue));
                    rowStyle.Setters.Add(new Setter(DataGridRow.FontWeightProperty, FontWeights.Bold));

                    dataGrid.Resources[typeof(DataGridRow)] = rowStyle;
                    dataGrid.Background = Brushes.Green;
                    dataGrid.Foreground = Brushes.Yellow;
                    dataGrid.BorderBrush = Brushes.Blue;
                    dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.None;
                    dataGrid.Padding = new Thickness(10);
                    dataGrid.BorderThickness = new Thickness(0);
                    dataGrid.Background = Brushes.Transparent;
                    dataGrid.ColumnHeaderStyle = Application.Current.FindResource("DataGridColumnHeaderStyle") as Style;
                    dataGrid.RowHeaderStyle = Application.Current.FindResource("DataGridRowHeaderStyle") as Style;
                }
                else if (child is TextBlock textBlock)
                {
                    textBlock.Background = Brushes.Red;
                    textBlock.Style = Application.Current.FindResource("TextBlockDefaultInfo") as Style;
                }
                else if (child is ToggleButton toggleButton)
                {
                    toggleButton.Background = Brushes.Red;
                    toggleButton.Style = Application.Current.FindResource("ToggleButtonSuccess") as Style;
                }
                else if (child is GridSplitter gridSplitter)
                {
                    gridSplitter.Background = Brushes.Transparent;
                }
                else if (child is Canvas canvas)
                {
                    canvas.Background = Brushes.Yellow;
                }
                else if (child is ListBox listBox)
                {

                    listBox.Style = Application.Current.FindResource("ListBoxBaseStyle") as Style;
                }
                else if (child is Border border)
                {
                    border.Background = Brushes.Red;
                }
                else if (child is Grid grid)
                {
                    var gc = grid.Children as UIElementCollection;
                    foreach (var c in gc)
                    {
                        if(c is StatusBar statusBar)
                        {
                            statusBar.Background = Brushes.Red;
                            foreach(StatusBarItem statusBarChild in LogicalTreeHelper.GetChildren(statusBar))
                            {
                                foreach (var statusBarItem in LogicalTreeHelper.GetChildren(statusBarChild))
                                {
                                    if (statusBarItem is Button button)
                                    {
                                        button.Background = Brushes.Yellow;
                                    }else if (statusBarItem is ComboBox comboBox)
                                    {
                                        comboBox.Style = Application.Current.FindResource("ComboBoxBaseStyle") as Style;
                                        foreach (var cbx in LogicalTreeHelper.GetChildren(comboBox))
                                        {
                                            Console.WriteLine("statusBarChild:" + cbx.GetType());
                                        }
                                    }
                                    else if (statusBarItem is ToggleButton tb)
                                    {
                                        tb.Foreground = Brushes.White;
                                        tb.Style = Application.Current.FindResource("ToggleButtonPrimary.Small") as Style;
                                    }
                                    Console.WriteLine("statusBarChild:" + statusBarItem.GetType());
                                }
                                
                            }
                        }
                        else if (c is AdornerDecorator adorner)
                        {
                            Console.WriteLine(66);
                        }else if (c is Grid gd)
                        {
                            Console.WriteLine("-----------------");
                            foreach (var gds in gd.Children as UIElementCollection)
                            {
                                if (gds is ScrollViewer sv)
                                {
                                    //sv.Background = Brushes.DarkRed;
                                }
                                
                                Console.WriteLine(gds.GetType().Name);
                            }
                            Console.WriteLine("-----------------");
                        }
                        else if (c is FrameworkElement element)
                        {
                            element.Margin = new Thickness(20);
                            element.Visibility= Visibility.Collapsed;
                            Console.WriteLine("FrameworkElement()");
                        }
                        //Console.WriteLine(c.GetType().Name);
                    }
                    //Console.WriteLine(grid.RowDefinitions.Count);
                    //Console.WriteLine(grid.ColumnDefinitions.Count);
                }
                else if (child is DependencyObject depChild)
                {
                    Console.WriteLine("类型：" + child.GetType().Name);
                    ApplyStylesUsingLogicalTree(depChild);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkflow(@"E:\VisualStudio\WorkFlowApp1\WorkFlowApp1\template\FlowChartActivity.xaml");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadWorkflow(@"E:\VisualStudio\WorkFlowApp1\WorkFlowApp1\template\SequenceActivity.xaml");
        }
    }
}
