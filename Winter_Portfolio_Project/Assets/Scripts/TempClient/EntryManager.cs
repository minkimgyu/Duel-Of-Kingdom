using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPP.Network;
using WPP.ClientInfo;

namespace WPP
{
    public class EntryManager : MonoBehaviour
    {
        [Header("Register")]
        public TMP_InputField registerID;
        public TMP_InputField registerPassword;
        public TMP_InputField registerRepeatPassword;
        public Button registerButton;

        [Header("Login")]
        public TMP_InputField loginID;
        public TMP_InputField loginPassword;
        public Button loginButton;
        void Start()
        {
            registerButton.onClick.AddListener(() => RegisterButtonOnClick(registerID.text, registerPassword.text, registerRepeatPassword.text));
            loginButton.onClick.AddListener(() => LoginButtonOnClick(loginID.text, loginPassword.text));
        }

        public void RegisterButtonOnClick(string username, string password, string repeatPassword)
        {
            if (!password.Equals(repeatPassword))
            {
                Debug.Log("password is not same");
                return;
            }
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteString(username);
            buffer.WriteString(password);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_REGISTER_ACCOUNT, buffer.ToArray());
            Debug.Log("Register Information sent");
        }

        public void LoginButtonOnClick(string username, string password)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteString(username);
            buffer.WriteString(password);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_LOGIN, buffer.ToArray());
            ClientData.Instance().account.username = username;
            ClientData.Instance().account.password = password;
            Debug.Log("Login Information sent");
        }
    }

}