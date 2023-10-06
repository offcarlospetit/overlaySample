using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OverlaySample
{	
	public partial class MyPage : ContentPage
	{	
		public MyPage ()
		{
			InitializeComponent ();
		}

        private void OnDocumentoClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CameraPage("Documento"));
        }

        private void OnDNIClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CameraPage("DNI"));
        }
    }
}

