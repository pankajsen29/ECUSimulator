using MessageDesignerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilityLib;
using Message = MessageDesignerLib.Message;

namespace ECUSim
{
    public partial class ECUSimMain : Form
    {
        public ECUSimMain()
        {
            InitializeComponent();
        }

        private void ECUSimMain_Load(object sender, EventArgs e)
        {
            LoadRequestResponseSubtab();
        }

        private async void LoadRequestResponseSubtab()
        {
            await LoadMessageDefinition();
            await LoadMessagesConfig();
        }
        private async Task LoadMessageDefinition()
        {
            //request json
            var pattern = new Pattern();
            pattern.Index = "";
            pattern.RangeStart = "";
            pattern.RangeEnd = "";

            var dataPatterns = new DataPatterns();
            dataPatterns.Pattern = new Pattern[] { pattern };

            var reqhexdata = new RequestHexData();
            reqhexdata.IsPatternBased = "false";
            reqhexdata.DataString = "41 81 C0 11 00 00 00 00";
            reqhexdata.DataPatterns = dataPatterns;

            var request = new Request();
            request.Id = "0x31";
            request.Payload = "8";
            request.IsCanFdFrame = "false";
            request.RequestHexData = reqhexdata;


            //response json string
            var substitution = new Substitution();
            substitution.SourceDataIndexFromRequest = "";
            substitution.DestinationDataIndexToResponse = "";

            var dataSubstitution = new DataSubstitutions();
            dataSubstitution.Substitution = new Substitution[] { substitution };

            var reshexdata = new ResponseHexData();
            reshexdata.IsPatternBased = "false";
            reshexdata.DataString = "41 81 C0 11 00 00 00 00";
            reshexdata.DataSubstitutions = dataSubstitution;

            var response = new Response();
            response.Id = "0x30";
            response.Payload = "8";
            response.IsCanFdFrame = "false";
            response.ResponseHexData = reshexdata;

            //message json string
            var message = new Message();
            message.Name = "cmd_1";
            message.Request = request;
            message.Response = response;

            txtMessage.Text = (string)(await JsonSerializationHelper.SerializeObject<Message>(message, true));
        }

        private async Task LoadMessagesConfig()
        {
            
        }
        private void btnAddMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
