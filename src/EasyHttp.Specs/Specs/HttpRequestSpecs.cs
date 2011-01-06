﻿#region License
// Distributed under the BSD License
// =================================
// 
// Copyright (c) 2010, Hadi Hariri
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Hadi Hariri nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// =============================================================
// 
// 
// Parts of this Software use JsonFX Serialization Library which is distributed under the MIT License:
// 
// Distributed under the terms of an MIT-style license:
// 
// The MIT License
// 
// Copyright (c) 2006-2009 Stephen M. McKamey
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Net;
using EasyHttp.Http;
using EasyHttp.Specs.Helpers;
using Machine.Specifications;

namespace EasyHttp.Specs.Specs
{
    [Subject("HttpClient")]
    public class when_making_any_type_of_request_to_invalid_host
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
        };

        Because of = () =>
        {
            exception = Catch.Exception( () => httpClient.Get("http://somethinginvalid") );

        };

        It should_throw_web_exception  = () =>
        {
            exception.ShouldBeOfType<WebException>();
            

        };

        static HttpClient httpClient;
        static Exception exception;
    }

    [Subject("HttpClient")]
    public class when_making_a_DELETE_request_with_a_valid_uri 
    {
        Establish context = () =>
        {
            httpClient = new HttpClient
                         {
                             Request = {Accept = HttpContentTypes.ApplicationJson}
                         };

            // First create customer in order to then delete it
            guid = Guid.NewGuid();

            httpClient.Put(string.Format("{0}/{1}", TestSettings.CouchDbDatabaseUrl, guid),
                          new Customer() {Name = "ToDelete", Email = "test@test.com"}, HttpContentTypes.ApplicationJson);

            response = httpClient.Response;

            rev = response.DynamicBody.rev;
        };

        Because of = () =>
        {

            httpClient.Delete(String.Format("{0}/{1}?rev={2}", TestSettings.CouchDbDatabaseUrl, guid, rev));
            response = httpClient.Response;
        };

        It should_delete_the_specified_resource = () =>
        {
            bool ok = response.DynamicBody.ok;

            string id = response.DynamicBody.id;

            ok.ShouldBeTrue();

            id.ShouldNotBeEmpty();

        };


        static HttpClient httpClient;
        static dynamic response;
        static string rev;
        static Guid guid;
    }

    [Subject("HttpClient")]
    public class when_making_a_GET_request_with_valid_uri
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
        };

        Because of = () =>
        {
            httpResponse = httpClient.Get("http://localhost:5984");

        };

        It should_return_body_with_rawtext =
            () => httpResponse.RawText.ShouldNotBeEmpty();

        static HttpClient httpClient;
        static HttpResponse httpResponse;
    }

    [Subject("HttpClient")]
    public class when_making_a_GET_request_with_valid_uri_and_content_type_set_to_application_json
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
            httpClient.Request.Accept = HttpContentTypes.ApplicationJson;

        };

        Because of = () =>
        {

            response = httpClient.Get(TestSettings.CouchDbRootUrl);

        };


        It should_return_dynamic_body_with_json_object = () =>
        {
            dynamic body = response.DynamicBody;

            string couchdb = body.couchdb;

            string version = body.version;

            couchdb.ShouldEqual("Welcome");

            version.ShouldNotBeEmpty();
        };


        It should_return_static_body_with_json_object = () =>
        {
            var couchInformation = response.StaticBody<CouchInformation>();

            couchInformation.message.ShouldEqual("Welcome");

            couchInformation.version.ShouldNotBeEmpty();
        };

        static HttpClient httpClient;
        static HttpResponse response;
    }

  
    [Subject("HttpClient")]
    public class when_making_a_HEAD_request_with_valid_uri
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
        };

        Because of = () =>
        {
            httpClient.Head(TestSettings.CouchDbRootUrl);
            httpResponse = httpClient.Response;

        };

        It should_return_OK_response  =
            () => httpResponse.StatusDescription.ShouldEqual("OK");

        static HttpClient httpClient;
        static HttpResponse httpResponse;
    }

    [Subject("HttpClient")]
    public class when_making_a_POST_request_with_valid_uri_and_valid_data_and_content_type_set_to_application_json
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
            httpClient.Request.Accept = HttpContentTypes.ApplicationJson;

        };

        Because of = () =>
        {

            httpClient.Post(TestSettings.CouchDbDatabaseUrl, new Customer() { Name = "Hadi", Email = "test@test.com" }, HttpContentTypes.ApplicationJson);

            response = httpClient.Response;
        };


        It should_succeed = () =>
        {
            bool ok = response.DynamicBody.ok;

            string id = response.DynamicBody.id;

            ok.ShouldBeTrue();

            id.ShouldNotBeEmpty();
        };



        static HttpClient httpClient;
        static dynamic response;
    }

    [Subject("HttpClient")]
    public class when_making_a_PUT_request_with_valid_uri_and_valid_data_and_content_type_set_to_application_json
    {
        Establish context = () =>
        {
            httpClient = new HttpClient();
            httpClient.Request.Accept = HttpContentTypes.ApplicationJson;
        };

        Because of = () =>
        {
            Guid guid = Guid.NewGuid();
            httpClient.Put(string.Format("{0}/{1}", TestSettings.CouchDbDatabaseUrl, guid),
                          new Customer() { Name = "Put", Email = "test@test.com" }, HttpContentTypes.ApplicationJson);

            response = httpClient.Response;
        };


        It should_succeed = () =>
        {
            bool ok = response.DynamicBody.ok;

            string id = response.DynamicBody.id;

            ok.ShouldBeTrue();

            id.ShouldNotBeEmpty();
        };



        static HttpClient httpClient;
        static dynamic response;
    }

  
}