using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MyCmn;

namespace MyWeb
{
    public partial class db : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEncrypte_Click(object sender, EventArgs e)
        {
            this.txtResult.Text = Security.EncryptString(this.txtContent.Text);

        }

        protected void btyDecrypte_Click(object sender, EventArgs e)
        {
            this.txtResult.Text = Security.DecrypteString(this.txtContent.Text);
        }
    }
}