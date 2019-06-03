using System.Text.RegularExpressions;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;

namespace Lab2
{
    class ContactInformation
    {
        public string adress { get; set; }
        public string phone { get; set; }
       public ContactInformation( string adress, string phone)
       {

            this.adress = adress;
            this.phone = phone;
       }
    }
    class Link : IComparable
    {
        public bool isRoot { get; set; }
        public bool isExternal { get; set; }
        public bool isHttps { get; set; }
        public bool hasWWW { get; set; }
        public string rootURI { get; set; }
        public string domain { get; set; }
        public string bodyURI { get; set; }
        public int depth { get; set; }

        public Link()
        {
            isHttps = false;
            hasWWW = true;
            rootURI = "";
            domain = "";
            bodyURI = "";
            depth = 0;
        }

        public Link(string link, bool _isExternal = false, bool _isRoot = false, int _depth = 0)
        {
            isRoot = _isRoot;
            isExternal = _isExternal;

            if (link.Contains("https"))
                isHttps = true;
            else
                isHttps = false;

            if (link.Contains("www"))
                hasWWW = true;
            else
                hasWWW = false;

            rootURI = obtainRoot(link);

            domain = obtainDomain(link, isRoot);

            bodyURI = obtainBody(link);

            depth = _depth;
        }

        public Link(string root, string _bodyURI, bool _isExternal = false, int _depth = 0)
        {
            isRoot = false;
            isExternal = _isExternal;

            if (root.Contains("https"))
                isHttps = true;
            else
                isHttps = false;

            if (root.Contains("www"))
                hasWWW = true;
            else
                hasWWW = false;

            rootURI = obtainRoot(root);

            domain = obtainDomain(root, true);

            bodyURI = obtainBody(_bodyURI);

            depth = _depth;
        }

        public static bool isLinkExternal(Link link, Link root)
        {
            if (link.rootURI == root.rootURI)
                return false;
            return true;
        }

        private string obtainRoot(string link)
        {
            int i = link.IndexOf("//");
            if (i >= 0)
            {
                if (hasWWW)
                    i += 6;
                else
                    i += 2;

                List<char> root = new List<char>();
                int n = link.LastIndexOf(".");
                int s = link.IndexOf("/", i);

                if (s >= 0 && s < n)
                    n = link.IndexOf(".", link.IndexOf(".") + 1);
                while (i < n)
                {
                    root.Add(link[i]);
                    i++;
                }

                string result = new string(root.ToArray());
                return result;
            }
            else
                return "";
        }

        private string obtainDomain(string link, bool isRoot = false)
        {
            int dotIndex = link.LastIndexOf(".");
            if (dotIndex >= 0 && link.Contains("."))
            {
                List<char> domain = new List<char>();
                if (!isRoot)
                {
                    int i = link.LastIndexOf(".") + 1;
                    int len = link.Length;
                    while (i < len && link[i] != '/')
                    {
                        domain.Add(link[i]);
                        i++;
                    }
                }
                else
                {
                    int n = link.Length;
                    if (link[n - 1] == '/')
                        n -= 1;

                    for (int i = dotIndex + 1; i < n; i++)
                        domain.Add(link[i]);
                }

                string result = new string(domain.ToArray());
                return result;
            }
            else
                return "";
        }

        private string obtainBody(string link)
        {
            int len = link.Length;
            if (!isRoot && link.Length > 0)
            {
                int first;
                if (link.Contains("//"))
                    first = link.IndexOf("/", link.IndexOf("//") + 2) + 1;
                else if (link[0] != '/')
                    first = 0;
                else
                    first = 1;

                if (first > 0)
                {
                    List<char> body = new List<char>();
                    for (int i = first; i < len; i++)
                    {
                        body.Add(link[i]);
                    }
                    if (body.Count != 0)
                    {
                        string result = new string(body.ToArray());
                        return result;
                    }
                }
                else if (link.Contains("http"))
                    return "";
                else
                    return link;
            }
            return "";
        }

        public int CompareTo(object obj)
        {
            try
            {
                Link other = (Link)obj;
                return (this.ToString().CompareTo(other.ToString()));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public override string ToString()
        {
            string http_s;
            if (isHttps)
                http_s = "https";
            else
                http_s = "http";

            string wwwDot;
            if (hasWWW)
                wwwDot = "www.";
            else
                wwwDot = "";

            return http_s + "://" + wwwDot + rootURI + "." + domain + "/" + bodyURI;
        }
    }

    class Analyzer
    {
        WebClient client;
        static SortedSet<Link> visitedLinks;
        static Stack<Link> currentPath;
        private string currentPage;
        private ContactInformation contact;
        public Link root { set; get; }
        public delegate void searchResult(Stack<Link> result, ContactInformation ci);
        public event searchResult onTarget;

        public Analyzer(string _root)
        {
            root = new Link(_root, false, true, 0);
            client = new WebClient();
            visitedLinks = new SortedSet<Link>();
            currentPath = new Stack<Link>();

            currentPath.Push(root);
        }

        public List<Link> findLinksOnPage(Link link, int depth)
        {
            List<Link> links = new List<Link>();
            try
            {
                currentPage = client.DownloadString(new Uri(link.ToString()));
                MatchCollection matches = Regex.Matches(currentPage, @"<a href=[""\/\w-\.:]+>");

                int size = matches.Count;
                for (int i = 0; i < size; i++)
                {
                    if (matches[i].ToString().Contains("http"))
                        links.Add(new Link(htmlLinkToURI(matches[i].ToString()), false, false, depth));
                    else
                        links.Add(new Link(root.ToString(), htmlLinkToURI(matches[i].ToString()), false, depth));
                }
                return links;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return links;
            }
        }
        public List<ContactInformation> findContactInformation(Link link)
        {
            List<ContactInformation> info = new List<ContactInformation>();
            try
            {
                currentPage = client.DownloadString(new Uri(link.ToString()));
                MatchCollection matches = Regex.Matches(currentPage, @"(8|\+7)?(\s)[\(](\d{3})[\)](\s)(\d{3})[\-](\d{2})[\-](\d{2})");
                MatchCollection matches2 = Regex.Matches(currentPage, @"/contact/(.*?)/susu/ru>"); 
              
                int max;
                if (matches.Count > matches2.Count)
                    max = matches.Count;    
                else
                    max = matches2.Count;
                for (int i = 0; i < matches.Count; i++)
                    Console.WriteLine(matches[i]);
                for (int i = 0; i< matches2.Count; i++)
                {                 
                    Console.Write(matches2[i].Groups[1]);
                    Console.WriteLine("@susu.ru");
                }
                for (int i = 0; i < max; i++)
                {
                    if(matches.Count > i)
                    info.Add(new ContactInformation(matches2[i].ToString(), matches[i].ToString()));
                    else
                        info.Add(new ContactInformation(matches2[i].ToString(), matches[matches.Count - 1].ToString()));
                }
                return info;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return info;
            }
        }
        private string htmlLinkToURI(string htmlLink)
        {
            //skips the <a href="/
            int i = 0;
            while (i < 9)
                i++;
            if (htmlLink[i] == '/') i++;

            string URI = "";

            while (htmlLink[i] != '>')
            {
                if (htmlLink[i] == '\"')
                    return URI;
                URI += htmlLink[i];
                i++;
            }
            return URI;
        }

        private List<Link> findExternalLinks(List<Link> links)
        {
            List<Link> external = new List<Link>();
            foreach (Link item in links)
                if (Link.isLinkExternal(item, root))
                {
                    external.Add(item);
                    item.isExternal = true;
                }
                else
                    item.isExternal = false;
            return external;
        }

        public void recSearch(Link thisLink, int maxDepth = 5, int maxPages = 1000, int depth = 0)
        {
            if (depth == maxDepth || visitedLinks.Count == maxPages)
                return;
            else if (!visitedLinks.Contains(thisLink))
            {
                Console.Write(thisLink.ToString());
                Console.Write(" - ");
                Console.Write(thisLink.depth);
                Console.Write(" - ");
                Console.WriteLine(visitedLinks.Count);

                visitedLinks.Add(thisLink);

                List<Link> links = findLinksOnPage(thisLink, depth + 1);
                List<ContactInformation> info = findContactInformation(thisLink);
                List<Link> external = findExternalLinks(links);
                if (external.Count != 0)
                    foreach (Link link in external)
                    {
                        currentPath.Push(link);
                        onTarget(currentPath, contact);
                        currentPath.Pop();
                    }

                if (links.Count != 0)
                {
                    foreach (Link link in links)
                    {
                        if (!link.isExternal)
                        {
                            currentPath.Push(link);
                            recSearch(link, maxDepth, maxPages, depth + 1);
                            if (currentPath.Count > 1)
                                currentPath.Pop();
                        }
                    }
                }
            }
        }

        public void visitedLinksCsvOut(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            StreamWriter w = new StreamWriter(file);
            foreach (Link link in visitedLinks)
            {
                w.Write(link.ToString());
                w.Write(";");
                w.WriteLine(link.depth);
            }
            w.Close();
        }
    }

    class AnalyzerHandler
    {
        string fileName;

        public AnalyzerHandler(string _csvFileName)
        {
            fileName = _csvFileName;
        }

        public void writeLinkCsv(Stack<Link> path, ContactInformation ci)
        {
            FileStream file = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter w = new StreamWriter(file);
            List<Link> links = new List<Link>(path);
            links.Reverse();

            foreach (Link link in links)
            {
                for (int i = 0; i < link.depth; i++)
                    w.Write(".");
                w.Write(" ");
                w.Write(link.ToString());
                w.Write(";");
                w.WriteLine(link.depth);
            }
            w.Close();
        }

        public void writeLinkConsole(Stack<Link> path, ContactInformation ci)
        {
            List<Link> links = new List<Link>(path);
            links.Reverse();
            foreach (Link link in links)
            {
                for (int i = 0; i < link.depth; i++)
                    Console.Write(" ");
                Console.Write(" ");
                Console.Write(link.ToString());
                Console.Write(" - ");
                Console.WriteLine(link.depth);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Analyzer a = new Analyzer("http://www.susu.ru/");
            AnalyzerHandler h = new AnalyzerHandler("links.csv");

            a.onTarget += h.writeLinkConsole;
            a.onTarget += h.writeLinkCsv;

            a.recSearch(a.root, 10, 100, 0);
            Console.WriteLine("--------------------------------------------------------------------------------END---------------------------------------------------------------------------------");
            a.visitedLinksCsvOut("visitedLinks.csv");
            Console.ReadKey();
        }
    }
}