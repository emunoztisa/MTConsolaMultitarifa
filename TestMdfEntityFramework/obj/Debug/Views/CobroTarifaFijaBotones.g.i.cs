﻿#pragma checksum "..\..\..\Views\CobroTarifaFijaBotones.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "AC119002322E000A2478CF4FD5F8AE6EA0AE4C833D4FF911B2A03BE117B62A69"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TestMdfEntityFramework.Views;


namespace TestMdfEntityFramework.Views {
    
    
    /// <summary>
    /// CobroTarifaFijaBotones
    /// </summary>
    public partial class CobroTarifaFijaBotones : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid1;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid2;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid popupGrid;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border popupBd;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtMensajePopup;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TestMdfEntityFramework;component/views/cobrotarifafijabotones.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            ((TestMdfEntityFramework.Views.CobroTarifaFijaBotones)(target)).Loaded += new System.Windows.RoutedEventHandler(this.CobroTarifaFijaBotones_OnLoad);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            ((TestMdfEntityFramework.Views.CobroTarifaFijaBotones)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.CobroTarifaFijaBotones_OnUnload);
            
            #line default
            #line hidden
            return;
            case 2:
            this.grid1 = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.grid2 = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.popupGrid = ((System.Windows.Controls.Grid)(target));
            
            #line 30 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            this.popupGrid.LostFocus += new System.Windows.RoutedEventHandler(this.popupGrid_LostFocus);
            
            #line default
            #line hidden
            
            #line 30 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            this.popupGrid.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.popupGrid_MouseDown);
            
            #line default
            #line hidden
            
            #line 30 "..\..\..\Views\CobroTarifaFijaBotones.xaml"
            this.popupGrid.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.popupGrid_TouchDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.popupBd = ((System.Windows.Controls.Border)(target));
            return;
            case 6:
            this.txtMensajePopup = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

