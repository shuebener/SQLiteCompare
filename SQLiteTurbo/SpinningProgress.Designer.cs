using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace CircularProgress.SpinningProgress
{
    // TRANSMISSINGCOMMENT: Class SpinningProgress
    partial class SpinningProgress : System.Windows.Forms.UserControl 
    { 
        
        // UserControl1 overrides dispose to clean up the component list.
        [ System.Diagnostics.DebuggerNonUserCode() ]
        protected override void Dispose( bool disposing ) 
        { 
            if ( disposing && components != null ) 
            { 
                components.Dispose(); 
            } 
            base.Dispose( disposing ); 
        } 
        
        
        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components; 
        
        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [ System.Diagnostics.DebuggerStepThrough() ]
        private void InitializeComponent() 
        { 
            this.SuspendLayout(); 
            // 
            // SpinningProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6.0F, 13.0F ); 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; 
            this.Name = "SpinningProgress"; 
            this.Size = new System.Drawing.Size( 30, 30 ); 
            this.ResumeLayout( false ); 
            
        } 
        
        
    } 
    
    
} 
