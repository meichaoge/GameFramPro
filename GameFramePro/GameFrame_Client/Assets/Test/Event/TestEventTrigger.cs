using GameFramePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //EventTrigger.RegisterMessageHandler<int, string, int, string>(10, (id, data, data2, data3, data4) =>
        //{
        //    Debug.Log($"收到消息 {id} : {data} {data.GetType()} {data2} {data3}{data4}");
        //});
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventTrigger.TriggerMessage(10, 100,"Hello44",88," bybye");
            EventTrigger.TriggerMessage(10, 100, "Hello33", 88);
            EventTrigger.TriggerMessage(10, 100, "Hello22");
            EventTrigger.TriggerMessage(10, 10);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            EventTrigger.RegisterMessageHandlerEx(10, typeof(int), typeof(string), typeof(int), typeof(string), MessageResponse_Ge4);
            EventTrigger.RegisterMessageHandlerEx(10, typeof(int), typeof(string), typeof(int), MessageResponse_Ge3);
            EventTrigger.RegisterMessageHandlerEx(10, typeof(int), typeof(string), MessageResponse_Ge2);
            EventTrigger.RegisterMessageHandlerEx(10, typeof(int),  MessageResponse_Ge1);


            //        EventTrigger.RegisterMessageHandler(10, MessageResponse);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            EventTrigger.UnRegisterMessageHandlerEx(10, typeof(int), typeof(string), typeof(int), typeof(string), MessageResponse_Ge4);
            EventTrigger.UnRegisterMessageHandlerEx(10, typeof(int), typeof(string), typeof(int), MessageResponse_Ge3);
            EventTrigger.UnRegisterMessageHandlerEx(10, typeof(int), typeof(string), MessageResponse_Ge2);
            EventTrigger.UnRegisterMessageHandlerEx(10, typeof(int),MessageResponse_Ge1);


            //    EventTrigger.UnRegisterMessageHandler(10, MessageResponse);
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            EventTrigger.RegisterMessageHandler(10, MessageResponse_Object4);

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventTrigger.UnRegisterMessageHandler(10, MessageResponse_Object4);

        }
    }


    private  void MessageResponse(int messageID,object data, object data2, object data3, object data4)
    {
        Debug.Log($"222收到消息 {messageID} : {data} {data.GetType()}   {data2}   {data3}  {data4}");
    }


    private void MessageResponse_Object4(int messageID, object data, object data2, object data3, object data4)
    {
        Debug.Log($"MessageResponse_Object4 收到消息 {messageID} : {data} {data.GetType()}   {data2}   {data3}  {data4}");
    }
    private void MessageResponse_Object3(int messageID, object data, object data2, object data3)
    {
        Debug.Log($"MessageResponse_Object3 收到消息 {messageID} : {data} {data.GetType()}   {data2}   {data3}  ");
    }
    private void MessageResponse_Object2(int messageID, object data, object data2)
    {
        Debug.Log($"MessageResponse_Object2 收到消息 {messageID} : {data} {data.GetType()}   {data2}  ");
    }
    private void MessageResponse_Object1(int messageID, object data)
    {
        Debug.Log($"MessageResponse_Object1 收到消息 {messageID} : {data} {data.GetType()}  ");
    }



    private void MessageResponse_Ge4(int messageID, object data, object data2, object data3, object data4)
    {
        Debug.Log($"MessageResponse_Ge4 收到消息 {messageID} : {data} {data.GetType()}   {data2}   {data3}  {data4}");
    }
    private void MessageResponse_Ge3(int messageID, object data, object data2, object data3)
    {
        Debug.Log($"MessageResponse_Ge3 收到消息 {messageID} : {data} {data.GetType()}   {data2}   {data3}  ");
    }
    private void MessageResponse_Ge2(int messageID, object data, object data2)
    {
        Debug.Log($"MessageResponse_Ge2 收到消息 {messageID} : {data} {data.GetType()}   {data2}  ");
    }
    private void MessageResponse_Ge1(int messageID, object data)
    {
        Debug.Log($"MessageResponse_Ge1 收到消息 {messageID} : {data} {data.GetType()}  ");
    }

}
