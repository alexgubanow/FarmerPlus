using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Xml.XPath;
using System.IO;

namespace farmer
{

    class xml
    {
        public static List<coordsformat> importcoords(string filename, bool mute)
        {
            List<coordsformat> coords = null;
            XDocument doc = XDocument.Load(filename);
            if (doc.XPathSelectElement("//ParcelMetricInfo").Elements("Externals").Any())
            {
                coords = new List<coordsformat>();
                int i = 0;
                XElement Externals = doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals");
                XElement ExternalsLines = Externals.XPathSelectElement("//Lines");
                foreach (XElement line in ExternalsLines.Elements())
                {
                    XElement Polyline = doc.XPathSelectElement("//Polyline");
                    foreach (XElement PL in Polyline.Elements())
                    {
                        if (PL.Element("ULID").Value == line.Element("ULID").Value)
                        {
                            XElement PointInfo = doc.XPathSelectElement("//PointInfo");
                            foreach (XElement Point in PointInfo.Elements())
                            {
                                if (PL.Element("Points").Elements().First().Value == Point.Element("UIDP").Value)
                                {
                                    coords.Add(new coordsformat() { x = Point.Element("X").Value, y = Point.Element("Y").Value, rank = i });
                                }
                            }
                        }
                    }
                }
                i++;
                if (doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals").Elements("Internals").Any())
                {
                    XElement Internals = doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals").Element("Internals");
                    foreach (XElement Boundary in Internals.Elements())
                    {
                        XElement Lines = Boundary.Element("Lines");
                        foreach (XElement line in Lines.Elements())
                        {
                            XElement Polyline = doc.XPathSelectElement("//Polyline");
                            foreach (XElement PL in Polyline.Elements())
                            {
                                if (PL.Element("ULID").Value == line.Element("ULID").Value)
                                {
                                    XElement PointInfo = doc.XPathSelectElement("//PointInfo");
                                    foreach (XElement Point in PointInfo.Elements())
                                    {
                                        if (PL.Element("Points").Elements().First().Value == Point.Element("UIDP").Value)
                                        {
                                            coords.Add(new coordsformat() { x = Point.Element("X").Value, y = Point.Element("Y").Value, rank = i });
                                        }
                                    }
                                }
                            }
                        }
                        i++;
                    }
                }
            }
            //if (doc.XPathSelectElement("//ParcelMetricInfo").Elements("Externals").Any())
            //{
            //    coords = new List<coordsformat>();
            //    List<string> listpoints = new List<string>();
            //    List<string> fpoints = new List<string>();
            //    List<string> spoints = new List<string>();
            //    int i = 0;
            //    foreach (XElement PL in doc.XPathSelectElement("//Polyline").Elements())
            //    {
            //        foreach (XElement p in PL.Elements())
            //        {
            //            if (p.Name == "Points")
            //            {
            //                fpoints.Add(p.Elements().First().Value);
            //                spoints.Add(p.Elements().Last().Value);
            //            }
            //        }
            //    }
            //    listpoints.Add(fpoints[0]);
            //    for (int l = 1; l < fpoints.Count; l++)
            //    {
            //        listpoints.Add(spoints[fpoints.IndexOf(listpoints[listpoints.Count-1])]);
            //    }
            //    XElement Externals = doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals");
            //    XElement ExternalsLines = Externals.XPathSelectElement("//Lines");
            //    for (int l = 0; l < listpoints.Count; l++)
            //    {
            //        XElement PointInfo = doc.XPathSelectElement("//PointInfo");
            //        foreach (XElement Point in PointInfo.Elements())
            //        {
            //            if (listpoints[l] == Point.Element("UIDP").Value)
            //            {
            //                coords.Add(new coordsformat() { x = Point.Element("X").Value, y = Point.Element("Y").Value, rank = i });
            //            }
            //        }
            //    }
            //    i++;
            //    if (doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals").Elements("Internals").Any())
            //    {
            //        XElement Internals = doc.XPathSelectElement("//ParcelMetricInfo").Element("Externals").Element("Internals");
            //        foreach (XElement Boundary in Internals.Elements())
            //        {
            //            XElement Lines = Boundary.Element("Lines");
            //            foreach (XElement line in Lines.Elements())
            //            {
            //                XElement Polyline = doc.XPathSelectElement("//Polyline");
            //                foreach (XElement PL in Polyline.Elements())
            //                {
            //                    if (PL.Element("ULID").Value == line.Element("ULID").Value)
            //                    {
            //                        XElement PointInfo = doc.XPathSelectElement("//PointInfo");
            //                        foreach (XElement Point in PointInfo.Elements())
            //                        {
            //                            if (PL.Element("Points").Elements().First().Value == Point.Element("UIDP").Value)
            //                            {
            //                                coords.Add(new coordsformat() { x = Point.Element("X").Value, y = Point.Element("Y").Value, rank = i });
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            i++;
            //        }
            //    }
            //}
            else
            {
                myvariable.errcount++;
                myvariable.errstr = myvariable.errstr +":"+ "coords";
                using (StreamWriter w = File.AppendText("erronimportxml.log"))
                {
                    w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                }
                if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "coords") != "0")
                {
                    MessageBox.Show("Нет координат");
                }
                return null;
            }
            return coords;
        }
        public static int importint(string filename, string elment, bool mute)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            switch (elment)
            {
                case "OwnershipInfo":
                    XmlNodeList OwnershipInfo = doc.GetElementsByTagName("OwnershipInfo");
                    string res = "";
                    for (int i = 0; i < OwnershipInfo[0].ChildNodes[0].InnerText.Length; i++)
                    {
                        if (Char.IsDigit(OwnershipInfo[0].ChildNodes[0].InnerText[i]) | OwnershipInfo[0].ChildNodes[0].InnerText[i] == '.')
                        {
                            res = res + OwnershipInfo[0].ChildNodes[0].InnerText[i];
                        }
                    }
                    switch (res)
                    {
                        case "100":
                            return 0;
                        case "200":
                            return 1;
                        case "300":
                            return 2;
                        default:
                            myvariable.errcount++;
                            myvariable.errstr = myvariable.errstr + ":" + "OwnershipInfo";
                            using (StreamWriter w = File.AppendText("erronimportxml.log"))
                            {
                                w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                            }
                            if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "OwnershipInfo") != "0")
                            {
                                MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " собственности:" +
                                    OwnershipInfo[0].ChildNodes[0].InnerText, (string)Application.Current.Resources["m_Error"]);
                            }
                            return -1;
                    }
                case "CategoryPurposeInfo":
                    XmlNodeList CategoryPurposeInfo = doc.GetElementsByTagName("Purpose");
                    res = "";
                    for (int i = 0; i < CategoryPurposeInfo[0].ChildNodes[0].InnerText.Length; i++)
                    {
                        if (Char.IsDigit(CategoryPurposeInfo[0].ChildNodes[0].InnerText[i]) | CategoryPurposeInfo[0].ChildNodes[0].InnerText[i] == '.')
                        {
                            res = res + CategoryPurposeInfo[0].ChildNodes[0].InnerText[i];
                        }
                    }
                    switch (res)
                    {
                        case "01.01":
                            return 0;
                        case "01.02":
                            return 1;
                        case "01.03":
                            return 2;
                        case "11.02":
                            return 3;
                        default:
                            myvariable.errcount++;
                            myvariable.errstr = myvariable.errstr + ":" + "CategoryPurposeInfo";
                            using (StreamWriter w = File.AppendText("erronimportxml.log"))
                            {
                                w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                            }
                            if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "CategoryPurposeInfo") != "0")
                            {
                                MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " целевом предназначении: " +
                                    CategoryPurposeInfo[0].ChildNodes[0].InnerText, (string)Application.Current.Resources["m_Error"]);
                            }
                            return -1;
                    }
                case "LandCode":
                    XmlNodeList LandCode = doc.GetElementsByTagName("LandCode");
                    res = "";
                    try
                    {
                        for (int i = 0; i < LandCode[0].ChildNodes[0].InnerText.Length; i++)
                        {
                            if (Char.IsDigit(LandCode[0].ChildNodes[0].InnerText[i]) | LandCode[0].ChildNodes[0].InnerText[i] == '.')
                            {
                                res = res + LandCode[0].ChildNodes[0].InnerText[i];
                            }
                        }
                    }
                    catch(Exception)
                    {
                        return -1;
                    }
                    switch (res)
                    {
                        case "001.01":
                            return 0;
                        case "001.00":
                            return 0;
                        case "002.01":
                            return 1;
                        case "002.02":
                            return 2;
                        case "002.03":
                            return 3;
                        case "013.00":
                            return 4;
                        case "011.00":
                            return 5;
                        default:
                            myvariable.errcount++;
                            myvariable.errstr = myvariable.errstr + ":" + "LandCode";
                            using (StreamWriter w = File.AppendText("erronimportxml.log"))
                            {
                                w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                            }
                            if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "LandCode") != "0")
                            {
                                MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " виде угодий: " + LandCode[0].ChildNodes[0].InnerText, (string)Application.Current.Resources["m_Error"]);
                            }
                            return -1;
                    }
            }
            myvariable.errcount++;
            myvariable.errstr = myvariable.errstr + ":" + "LandCode";
            using (StreamWriter w = File.AppendText("erronimportxml.log"))
            {
                w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
            }
            return -1;
        }
        public static string importtext(string filename, string elment, bool mute)
        {
            XmlDocument doc = new XmlDocument();
            XDocument xdoc = XDocument.Load(filename);
            doc.Load(filename);
            switch (elment)
            {
                case "LeaseDuration":
                    XmlNodeList LeaseAgreement = doc.GetElementsByTagName("LeaseAgreement");
                    if (LeaseAgreement.Count != 0)
                    {
                        string leaseduration = "";
                        for (int j = 0; j < LeaseAgreement[0].ChildNodes[2].InnerText.Length; j++)
                        {
                            if (Char.IsDigit(LeaseAgreement[0].ChildNodes[2].InnerText[j]) | LeaseAgreement[0].ChildNodes[2].InnerText[j].Equals('.'))
                            {
                                leaseduration = leaseduration + LeaseAgreement[0].ChildNodes[2].InnerText[j];
                            }
                        }
                        return leaseduration;
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "LeaseDuration";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "LeaseDuration") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " аренде", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                case "KOATUU":
                    XmlNodeList KOATUU = doc.GetElementsByTagName("KOATUU");
                    if (KOATUU.Count != 0)
                    {
                        return KOATUU[0].InnerText;
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "KOATUU";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "KOATUU") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " КОАТУУ", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                case "RegistrationDate":
                    XmlNodeList RegistrationDate = doc.GetElementsByTagName("LeaseAgreement");
                    if (RegistrationDate.Count != 0)
                    {
                        DateTime resul;
                        if (DateTime.TryParse(RegistrationDate[0].ChildNodes[5].InnerText, out resul))
                        {
                            return resul.ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        return "";
                    }
                case "typeGrantee":
                    XElement LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                    if (LegalModeInfo != null)
                    {
                        foreach (XElement contractor in LegalModeInfo.Elements())
                        {
                            if (contractor.Name == "Grantee")
                            {
                                foreach (XElement type in contractor.Elements())
                                {
                                    return type.Name.LocalName;
                                }
                            }
                        }
                    }
                    return "";
                case "typeGrantor":
                    LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                    if (LegalModeInfo != null)
                    {
                        XElement Grantor = LegalModeInfo.Element("Authentication");
                        foreach (XElement type in Grantor.Elements())
                        {
                            return type.Name.LocalName;
                        }
                    }
                    myvariable.errcount++;
                    myvariable.errstr = myvariable.errstr + ":" + "typeGrantor";
                    using (StreamWriter w = File.AppendText("erronimportxml.log"))
                    {
                        w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                    }
                    return "";
                case "CadastralCode":
                    XmlNodeList CadastralCode = doc.GetElementsByTagName("CadastralCode");
                    if (CadastralCode.Count != 0)
                    {
                        return CadastralCode[0].InnerText;
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "CadastralCode";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "CadastralCode") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " кадастровом номере", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                case "Size":
                    XElement ParcelMetricInfo = xdoc.XPathSelectElement("//ParcelMetricInfo");
                    if (ParcelMetricInfo != null)
                    {
                        foreach (XElement block in ParcelMetricInfo.Elements())
                        {
                            if (block.Name == "Area")
                            {
                                foreach (XElement row in block.Elements())
                                {
                                    if (row.Name == "Size")
                                    {
                                        return row.Value;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "Size";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "Size") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " площади", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                    myvariable.errcount++;
                    myvariable.errstr = myvariable.errstr + ":" + "Size";
                    using (StreamWriter w = File.AppendText("erronimportxml.log"))
                    {
                        w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                    }
                    return "";
                case "LandsParcel":
                    XmlNodeList LandsParcel = doc.GetElementsByTagName("LandsParcel");
                    if (LandsParcel.Count != 0)
                    {
                        return LandsParcel[0].ChildNodes.Count.ToString();
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "LandsParcel";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "LandsParcel") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " угодях", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                case "District":
                    XmlNodeList District = doc.GetElementsByTagName("ParcelLocationInfo");
                    if (District[0].ChildNodes.Count != 0)
                    {
                        return District[0].ChildNodes[2].InnerText;
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "District";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "District") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " районе", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
                case "Settlement":
                    XmlNodeList Settlement = doc.GetElementsByTagName("ParcelLocationInfo");
                    if (Settlement[0].ChildNodes.Count != 0)
                    {
                        return Settlement[0].ChildNodes[1].InnerText;
                    }
                    else
                    {
                        myvariable.errcount++;
                        myvariable.errstr = myvariable.errstr + ":" + "Settlement";
                        using (StreamWriter w = File.AppendText("erronimportxml.log"))
                        {
                            w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
                        }
                        if (!mute && IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", "Settlement") != "0")
                        {
                            MessageBox.Show((string)Application.Current.Resources["m_xmllessinfoabout"] + " сельсовете", (string)Application.Current.Resources["m_Error"]);
                        }
                        return "";
                    }
            }
            myvariable.errcount++;
            myvariable.errstr = myvariable.errstr + ":" + "Settlement";
            using (StreamWriter w = File.AppendText("erronimportxml.log"))
            {
                w.WriteLine(DateTime.Now.ToString("dd.MM.yyyy_HH:mm:ss\t" + Path.GetFileName(filename)));
            }
            return "";
        }
        public static string importcontractorinf(string filename, string elment, string side, bool mute)
        {
            XDocument xdoc = XDocument.Load(filename);
            switch (side)
            {
               case "Grantor":
                    switch (elment)
                    {
                        case "NaturalPerson_FullName":
                            XElement LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "FullName")
                                            {
                                                string fullname = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    fullname = fullname + n.FirstNode.ToString() + ' ';
                                                }
                                                return fullname;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_TaxNumber":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "TaxNumber")
                                            {
                                                if (elem.FirstNode != null)
                                                {
                                                    return elem.FirstNode.ToString();
                                                }
                                                else
                                                {
                                                    return "";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_Passport":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Passport")
                                            {
                                                string PassportNumber = "";
                                                string PassportSeries = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null)
                                                    {
                                                        if (n.Name.LocalName == "PassportNumber")
                                                        {
                                                            PassportNumber = n.FirstNode.ToString();
                                                        }
                                                        else if (n.Name.LocalName == "PassportSeries")
                                                        {
                                                            PassportSeries = n.FirstNode.ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return "";
                                                    }
                                                }
                                                return PassportSeries + PassportNumber;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_PassportIssuedDate":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem != null && elem.Name == "Passport")
                                            {
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.Name.LocalName == "PassportIssuedDate")
                                                    {
                                                        if (n.FirstNode != null)
                                                        {
                                                            return n.FirstNode.ToString();
                                                        }
                                                        else
                                                        {
                                                            return "";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_IssuanceAuthority":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem != null && elem.Name == "Passport")
                                            {
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.Name.LocalName == "IssuanceAuthority")
                                                    {
                                                        if (n.FirstNode != null)
                                                        {
                                                            return n.FirstNode.ToString();
                                                        }
                                                        else
                                                        {
                                                            return "";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_Address":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Address")
                                            {
                                                string Address = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null)
                                                    {
                                                        if (n.Name.LocalName != "Country")
                                                        {
                                                            Address = Address + n.FirstNode.ToString() + ", ";
                                                        }
                                                    }
                                                }
                                                if (Address != "")
                                                {
                                                    return Address.Remove(Address.Length - 2, 2);
                                                }
                                                else
                                                {
                                                    return "";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_Name":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Name")
                                            {
                                                if (elem.FirstNode != null)
                                                {
                                                    return elem.FirstNode.ToString();
                                                }
                                                else
                                                {
                                                    return "";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_EDRPOU":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "EDRPOU")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_Address":
                            LegalModeInfo = xdoc.XPathSelectElement("//ProprietorInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantor = LegalModeInfo.Element("Authentication");
                                if (Grantor != null)
                                {
                                    XElement NaturalPerson = Grantor.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Address")
                                            {
                                                string Address = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null && n.Name.LocalName != "Country")
                                                    {
                                                        Address = Address + n.FirstNode.ToString() + ", ";
                                                    }
                                                }
                                                return Address.Remove(Address.Length - 2, 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                    }
                    return "";
                case "Grantee":
                    switch (elment)
                    {
                        case "NaturalPerson_FullName":
                            XElement LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "FullName")
                                            {
                                                string fullname = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    fullname = fullname + n.FirstNode.ToString() + ' ';
                                                }
                                                return fullname;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_TaxNumber":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "TaxNumber")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_Passport":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Passport")
                                            {
                                                string PassportNumber = "";
                                                string PassportSeries = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null)
                                                    {
                                                        if (n.Name.LocalName == "PassportNumber")
                                                        {
                                                            PassportNumber = n.FirstNode.ToString();
                                                        }
                                                        else if (n.Name.LocalName == "PassportSeries")
                                                        {
                                                            PassportSeries = n.FirstNode.ToString();
                                                        }
                                                    }
                                                }
                                                return PassportSeries + PassportNumber;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_PassportIssuedDate":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "PassportIssuedDate")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_IssuanceAuthority":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "IssuanceAuthority")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "NaturalPerson_Address":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("NaturalPerson");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Address")
                                            {
                                                string Address = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null && n.Name.LocalName != "Country")
                                                    {
                                                        Address = Address + n.FirstNode.ToString() + ", ";
                                                    }
                                                }
                                                return Address.Remove(Address.Length - 2, 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_Name":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Name")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_EDRPOU":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "EDRPOU")
                                            {
                                                return elem.FirstNode.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                        case "LegalEntity_Address":
                            LegalModeInfo = xdoc.XPathSelectElement("//LegalModeInfo");
                            if (LegalModeInfo != null)
                            {
                                XElement Grantee = LegalModeInfo.Element("Grantee");
                                if (Grantee != null)
                                {
                                    XElement NaturalPerson = Grantee.Element("LegalEntity");
                                    if (NaturalPerson != null)
                                    {
                                        foreach (XElement elem in NaturalPerson.Elements())
                                        {
                                            if (elem.Name == "Address")
                                            {
                                                string Address = "";
                                                foreach (XElement n in elem.Elements())
                                                {
                                                    if (n.FirstNode != null && n.Name.LocalName != "Country")
                                                    {
                                                        Address = Address + n.FirstNode.ToString() + ", ";
                                                    }
                                                }
                                                return Address.Remove(Address.Length - 2, 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }
                            return "";
                    }
                    return "";
            }
            return "";
        }
    }
}
