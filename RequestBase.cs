using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using com.adjust.sdk;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;

public class RequestBase
{
    protected string _uri = Constant.HOST_NOR;
    protected string uri
    {
        get
        {
#if PRE
            _uri = Constant.HOST_PRE;
#elif DEV
            _uri = Constant.HOST_DEV;
#endif
            return _uri;
        }
    }

    protected UnityWebRequest request;

    public RequestBase(string endPoin)
    {
        this.request = UnityWebRequest.Get(uri + endPoin);
    }

    string dataPost;
    public RequestBase(string endPoin, string postData)
    {
        //this.request = UnityWebRequest.Put(uri + endPoin, postData);


        if (!string.IsNullOrEmpty(postData))
        {
            dataPost = postData;
            this.request = new UnityWebRequest(uri + endPoin, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        }
        else
            this.request = UnityWebRequest.Post(uri + endPoin, "");
        this.request.SetRequestHeader("Content-Type", "application/json");

        this.request.SetRequestHeader("Accept", "application/json");
    }


    public RequestBase setHeader(string name, string value)
    {
        this.request.SetRequestHeader(name, value);

        return this;
    }


    public async Task<RequestBase> Send(System.Action<RequestBase> onDone = null)
    {
        //Debug.LogError(this.request.uri+ " ---- ");

        this.request.SendWebRequest();

        while (!this.request.isDone)
        {
            await Task.Delay(10);
        }

        Debug.LogError(this.request.uri+"----"+dataPost+ " ---- "+this.responseCode+"-----" + this.response);

        if (!string.IsNullOrEmpty(access_token))
        {
            Debug.LogError("=======>"+access_token);
            UserDatas.access_token = access_token;
            UserDatas.SaveLocalData();
        }

        if (onDone != null)
        {
            onRequestComplete(onDone);
        }

        // Request and wait for the desired page.
        return this;
    }


    void onRequestComplete(System.Action<RequestBase> action)
    {
        action(this);
    }

    public bool isDone => this.request.isDone;

    public float progress => this.request.downloadProgress;

    public string access_token => this.request.GetResponseHeader("x-access-token");
    public string response => this.request.downloadHandler.text;

    public long responseCode => this.request.responseCode;

    public enum RequestType
    {
        GET,
        POST
    }
}
