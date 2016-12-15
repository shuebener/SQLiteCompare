using System;
using System.Threading;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// Submits post data to a url.
    /// </summary>
    public class PostSubmitter
    {
        /// <summary>
        /// determines what type of post to perform.
        /// </summary>
        public enum PostTypeEnum
        {
            /// <summary>
            /// Does a get against the source.
            /// </summary>
            Get,
            /// <summary>
            /// Does a post against the source.
            /// </summary>
            Post
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostSubmitter()
        {
        }

        /// <summary>
        /// Constructor that accepts a url as a parameter
        /// </summary>
        /// <param name="url">The url where the post will be submitted to.</param>
        public PostSubmitter(string url)
            : this()
        {
            m_url = url;
        }

        /// <summary>
        /// Constructor allowing the setting of the url and items to post.
        /// </summary>
        /// <param name="url">the url for the post.</param>
        /// <param name="values">The values for the post.</param>
        public PostSubmitter(string url, NameValueCollection values)
            : this(url)
        {
            m_values = values;
        }

        /// <summary>
        /// Gets or sets the url to submit the post to.
        /// </summary>
        public string Url
        {
            get
            {
                return m_url;
            }
            set
            {
                m_url = value;
            }
        }
        /// <summary>
        /// Gets or sets the name value collection of items to post.
        /// </summary>
        public NameValueCollection PostItems
        {
            get
            {
                return m_values;
            }
            set
            {
                m_values = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of action to perform against the url.
        /// </summary>
        public PostTypeEnum Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Cancels an on-going POST operation
        /// </summary>
        public void CancelPost()
        {
            lock (this)
            {
                if (_worker != null && _worker.IsAlive)
                    _worker.Interrupt();
            } // lock
        }

        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <returns>a string containing the result of the post.</returns>
        public string Post()
        {
            StringBuilder parameters = new StringBuilder();
            for (int i = 0; i < m_values.Count; i++)
            {
                EncodeAndAddItem(ref parameters, m_values.GetKey(i), m_values[i]);
            }
            string result = PostData(m_url, parameters.ToString());
            return result;
        }

        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to post to.</param>
        /// <returns>a string containing the result of the post.</returns>
        public string Post(string url)
        {
            m_url = url;
            return this.Post();
        }

        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to post to.</param>
        /// <param name="values">The values to post.</param>
        /// <returns>a string containing the result of the post.</returns>
        public string Post(string url, NameValueCollection values)
        {
            m_values = values;
            return this.Post(url);
        }

        /// <summary>
        /// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
        /// </summary>
        /// <param name="postData">The data to post.</param>
        /// <param name="url">the url to post to.</param>
        /// <returns>Returns the result of the post.</returns>
        private string PostData(string url, string postData)
        {
            lock (this)
            {
                if (_busy)
                    throw new InvalidOperationException("Another PostData operation is active");
                _busy = true;
            } // lock

            string result = string.Empty;
            bool cancelled = false;
            Exception error = null;

            _worker = new Thread(new ThreadStart(delegate
            {
                try
                {
                    HttpWebRequest request = null;
                    if (m_type == PostTypeEnum.Post)
                    {
                        Uri uri = new Uri(url);
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Timeout = 10000;
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = postData.Length;
                        using (Stream writeStream = request.GetRequestStream())
                        {
                            UTF8Encoding encoding = new UTF8Encoding();
                            byte[] bytes = encoding.GetBytes(postData);
                            writeStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                    else
                    {
                        Uri uri = new Uri(url + "?" + postData);
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Method = "GET";
                    }

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                result = readStream.ReadToEnd();
                            }
                        }
                    }
                }
                catch (ThreadInterruptedException tex)
                {
                    cancelled = true;
                } // catch
                catch (Exception ex)
                {
                    error = ex;
                }
            }));

            try
            {
                _worker.IsBackground = true;
                _worker.Name = "PostData Thread";
                _worker.Start();

                // Wait for the worker thread to complete
                _worker.Join();

                if (cancelled)
                    throw new UserCancellationException();

                if (error != null)
                    throw error;
            }
            finally
            {
                lock (this)
                    _busy = false;
            } // finally

            return result;
        }

        /// <summary>
        /// Encodes an item and ads it to the string.
        /// </summary>
        /// <param name="baseRequest">The previously encoded data.</param>
        /// <param name="dataItem">The data to encode.</param>
        /// <returns>A string containing the old data and the previously encoded data.</returns>
        private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
        {
            if (baseRequest == null)
            {
                baseRequest = new StringBuilder();
            }
            if (baseRequest.Length != 0)
            {
                baseRequest.Append("&");
            }
            baseRequest.Append(key);
            baseRequest.Append("=");
            baseRequest.Append(System.Web.HttpUtility.UrlEncode(dataItem));
        }

        #region Private Variables
        private string m_url = string.Empty;
        private NameValueCollection m_values = new NameValueCollection();
        private PostTypeEnum m_type = PostTypeEnum.Get;
        private Thread _worker = null;
        private bool _busy = false;
        #endregion
    }
}