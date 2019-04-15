using BoardApi.Controllers;
using BoardApi.Model;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class MessageBoardControllerTest
    {
        private List<Message> GetTestMessages()
        {
            var list = new List<Message>();

            for (var i = 0; i < 10; i++)
            {
                var msg = new Message
                {
                    Id = i,
                    Text = "abc" + i.ToString(),
                    UserId = i + 1,
                    Date = DateTime.Now
                };
                list.Add(msg);
            }

            return list;
        }

        [TestCase]
        public void GetAllMessages_AssertAllMessagesReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var response = controller.Get();
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var x = result as OkObjectResult;
            var list = x.Value as List<Message>;
            Assert.IsTrue(list.Count == messages.Count);
        }

        [TestCase]
        public void PostMessage_AssertOkReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var msgId = 15;
            var userId = 2;
            var text = "Hello World";
            var message = new Message
            {
                Text = text,
                UserId = userId
            };
            var response = controller.Put(msgId, message);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(NotFoundResult));
        }

        [TestCase]
        public void DeleteMessageNotExisting_AssertNotFoundReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var userId = 2;
            var response = controller.Delete(15, userId);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(NotFoundResult));
        }

        [TestCase]
        public void DeleteMessageExisting_AssertDeletedMessageReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var msgId = 2;
            var userId = 2;
            var response = controller.Delete(msgId, userId);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var x = result as OkObjectResult;
            var msg = x.Value as Message;
            Assert.IsTrue(msg.IsDeleted == true);
            Assert.IsTrue(msg.Id == msgId);
        }

        [TestCase]
        public void DeleteMessageWrongInput_AssertBadResponseReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var msgId = 1;
            var userId = -1;
            var response = controller.Delete(msgId, userId);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(BadRequestObjectResult));
        }

        [TestCase]
        public void UpdateMessageNotExisting_AssertErrorReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var msgId = 15;
            var userId = 2;
            var text = "Hello World";
            var message = new Message
            {
                Text = text,
                UserId = userId
            };
            var response = controller.Put(msgId, message);
            var result = response.Result;
            
            Assert.IsTrue(result.GetType() == typeof(NotFoundResult));
        }

        [TestCase]
        public void UpdateMessageExisting_AssertUpdatedMessageReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var msgId = 2;
            var userId = 2;
            var text = "Hello World";
            var message = new Message
            {
                Text = text,
                UserId = userId
            };
            var response = controller.Put(msgId, message);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var x = result as OkObjectResult;
            var msg = x.Value as Message;
            Assert.IsTrue(msg.Text == text);
            Assert.IsTrue(msg.Id == msgId);
        }

        [TestCase]
        public void UpdateMessageWrongInput_AssertBadResponseReturned()
        {
            var messages = GetTestMessages();
            var controller = new MessageBoardController(messages);
            var response = controller.Put(-1, null);
            var result = response.Result;

            Assert.IsTrue(result.GetType() == typeof(BadRequestResult));
        }
    }
}