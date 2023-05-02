using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpTest01.AnimalHpt;
using wpTest01.Logics;

namespace wpTest01
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false;
        public MainWindow()
        {
            InitializeComponent();
            do
            {
                QueryHosp();
            } while (false);
        }
        #region < 실행시 데이터 조회 >
        private async void QueryHosp()
        {
            string data_apiKey = "YYHJjeiagyFeSD69gRR9ykzP7KS0HqJOPMzv0HmJ8ETzz2witGrEIA63UsK40QjbpFfTSI2lRJ23P3%2FsHnjoRw%3D%3D";
            // string encoding_hospName = HttpUtility.UrlEncode(hospName, Encoding.UTF8);
            string openApiUrl = $"https://apis.data.go.kr/6260000/BusanAnimalHospService/getTblAnimalHospital?serviceKey={data_apiKey}&pageNo=1&numOfRows=1000&resultType=json";
            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUrl);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                //Debug.WriteLine(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            var jsonResult = JObject.Parse(result); // string => json
            var resultCode = Convert.ToString(jsonResult["getTblAnimalHospital"]["header"]["resultCode"]);

            if (resultCode == "00") // 코드 00 : 정상
            {
                var data = jsonResult["getTblAnimalHospital"]["body"]["items"]["item"];
                var json_array = data as JArray;

                var hosplist = new List<Hosp>();
                foreach (var list in json_array)
                {
                    if (Convert.ToString(list["animal_hospital"]) != string.Empty)
                    {

                    //    if (Convert.ToString(list["animal_hospital"]).Contains(hospName))
                    //    {
                            hosplist.Add(new Hosp
                            {
                                gugun = Convert.ToString(list["gugun"]),
                                animal_hospital = Convert.ToString(list["animal_hospital"]),
                                approval = Convert.ToDateTime(list["approval"]),
                                road_address = Convert.ToString(list["road_address"]),
                                tel = Convert.ToString(list["tel"]),
                                lat = Convert.ToString(list["lat"]) == string.Empty ? 0.0 : Convert.ToDouble(list["lat"]),
                                lon = Convert.ToString(list["lon"]) == string.Empty ? 0.0 : Convert.ToDouble(list["lon"]),
                                basic_date = Convert.ToDateTime(list["basic_date"])
                            });
                    //    }
                    }
                }
                this.DataContext = hosplist;
                isFavorite = false;
                StsResult.Content = $"동물병원 검색결과 {hosplist.Count}건 조회완료";
            }
        }
        #endregion

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtHptName.Focus();
            #region < 콤보박스 부분 >
            string data_apiKey = "YYHJjeiagyFeSD69gRR9ykzP7KS0HqJOPMzv0HmJ8ETzz2witGrEIA63UsK40QjbpFfTSI2lRJ23P3%2FsHnjoRw%3D%3D";
            // string encoding_hospName = HttpUtility.UrlEncode(hospName, Encoding.UTF8);
            string openApiUrl = $"https://apis.data.go.kr/6260000/BusanAnimalHospService/getTblAnimalHospital?serviceKey={data_apiKey}&pageNo=1&numOfRows=1000&resultType=json";
            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUrl);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                //Debug.WriteLine(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            var jsonResult = JObject.Parse(result); // string => json
            var resultCode = Convert.ToString(jsonResult["getTblAnimalHospital"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00") // 코드 00 : 정상
                {
                    var data = jsonResult["getTblAnimalHospital"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var hosplist = new HashSet<string>();
                    foreach (var list in json_array)
                    {
                        if (Convert.ToString(list["gugun"]) != string.Empty) // == string.Empty
                        {
                            hosplist.Add(Convert.ToString(list["gugun"]));
                        }
                    }
                    hosplist.Add("");
                    Debug.WriteLine(hosplist);
                    CboSelectArea.ItemsSource = hosplist;
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            }

            #endregion
            #region < 실행시 데이터 삭제, 등록 >
            using (SqlConnection conn = new SqlConnection(Commons.connString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                var query = "DELETE FROM AnimalHosp WHERE animal_hospital = @animal_hospital";
                var delRes = 0;

                foreach (Hosp item in GrdResult.Items)
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@animal_hospital", item.animal_hospital);

                    delRes += cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection conn = new SqlConnection(Commons.connString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                var query = @"INSERT INTO AnimalHosp
                                             (
                                              gugun
                                             ,animal_hospital
                                             ,approval
                                             ,road_address
                                             ,tel
                                             ,lat
                                             ,lon
                                             ,basic_date)
                                       VALUES
                                             (
                                              @gugun 
                                             ,@animal_hospital
                                             ,@approval 
                                             ,@road_address 
                                             ,@tel
                                             ,@lat
                                             ,@lon
                                             ,@basic_date)";

                var insRes = 0;
                foreach (Hosp item in GrdResult.Items)
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@gugun", item.gugun);
                    cmd.Parameters.AddWithValue("@animal_hospital", item.animal_hospital);
                    cmd.Parameters.AddWithValue("@approval", item.approval);
                    cmd.Parameters.AddWithValue("@road_address", item.road_address);
                    cmd.Parameters.AddWithValue("@tel", item.tel);
                    cmd.Parameters.AddWithValue("@lat", item.lat);
                    cmd.Parameters.AddWithValue("@lon", item.lon);
                    cmd.Parameters.AddWithValue("@basic_date", item.basic_date);

                    insRes += cmd.ExecuteNonQuery();
                }
            }
            #endregion
        }

        #region < 검색 함수 부분> 
        private void TxtHptName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchHpt_Click(sender, e);
            }
        }

        private async void BtnSearchHpt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtHptName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 동물병원 이름을 입력하세요.");
                return; 
            }
            try
            {
                SearchHosp(TxtHptName.Text, CboSelectArea.SelectedValue);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }
        #endregion

        #region < 실제 검색 하는 부분 >
        private void SearchHosp(string hospName, object gugunName)
        {
            if (gugunName == null)
            {
                gugunName = string.Empty;
            }
            // MessageBox.Show(CboReqDate.SelectedValue.ToString());
            using (SqlConnection conn = new SqlConnection(Commons.connString))
            {
                conn.Open();
                var query = @"SELECT gugun,
                                     animal_hospital,
                                     approval,
                                     road_address,
                                     tel,
                                     lat,
                                     lon,
                                     basic_date
                                FROM AnimalHosp
                               WHERE gugun LIKE '%' + @gugun +'%'
                                 AND animal_hospital LIKE @animal_hospital +'%'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gugun", gugunName);
                cmd.Parameters.AddWithValue("@animal_hospital", hospName);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "AnimalHosp");
                List<Hosp> hospList = new List<Hosp>();
                foreach (DataRow row in ds.Tables["AnimalHosp"].Rows)
                {
                    hospList.Add(new Hosp
                    {
                        gugun = Convert.ToString(row["gugun"]),
                        animal_hospital = Convert.ToString(row["animal_hospital"]),
                        approval = Convert.ToDateTime(row["approval"]),
                        road_address = Convert.ToString(row["road_address"]),
                        tel = Convert.ToString(row["tel"]),
                        lat = Convert.ToDouble(row["lat"]),
                        lon = Convert.ToDouble(row["lon"]),
                        basic_date = Convert.ToDateTime(row["basic_date"]),
                    });
                }

                this.DataContext = hospList;
                StsResult.Content = $"DB {hospList.Count}건 조회완료";
            }
            
            
            //string data_apiKey = "YYHJjeiagyFeSD69gRR9ykzP7KS0HqJOPMzv0HmJ8ETzz2witGrEIA63UsK40QjbpFfTSI2lRJ23P3%2FsHnjoRw%3D%3D";
            //// string encoding_hospName = HttpUtility.UrlEncode(hospName, Encoding.UTF8);
            //string openApiUrl = $"https://apis.data.go.kr/6260000/BusanAnimalHospService/getTblAnimalHospital?serviceKey={data_apiKey}&pageNo=1&numOfRows=1000&resultType=json";
            //string result = string.Empty;

            //WebRequest req = null;
            //WebResponse res = null;
            //StreamReader reader = null;

            //try
            //{
            //    req = WebRequest.Create(openApiUrl);
            //    res = await req.GetResponseAsync();
            //    reader = new StreamReader(res.GetResponseStream());
            //    result = reader.ReadToEnd();

            //    //Debug.WriteLine(result);
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            //finally
            //{
            //    reader.Close();
            //    res.Close();
            //}

            //var jsonResult = JObject.Parse(result); // string => json
            //var resultCode = Convert.ToString(jsonResult["getTblAnimalHospital"]["header"]["resultCode"]);

            //try
            //{
            //    if (resultCode == "00") // 코드 00 : 정상
            //    {
            //        var data = jsonResult["getTblAnimalHospital"]["body"]["items"]["item"];
            //        var json_array = data as JArray;

            //        var hosplist = new List<Hosp>();
            //        foreach (var list in json_array)
            //        {
            //            if (Convert.ToString(list["animal_hospital"]) != string.Empty)
            //            {

            //                if (Convert.ToString(list["animal_hospital"]).Contains(hospName))
            //                {
            //                    hosplist.Add(new Hosp
            //                    {
            //                        gugun = Convert.ToString(list["gugun"]),
            //                        animal_hospital = Convert.ToString(list["animal_hospital"]),
            //                        approval = Convert.ToDateTime(list["approval"]),
            //                        road_address = Convert.ToString(list["road_address"]),
            //                        tel = Convert.ToString(list["tel"]),
            //                        lat = Convert.ToString(list["lat"]) == string.Empty ? 0.0 :Convert.ToDouble(list["lat"]),
            //                        lon = Convert.ToString(list["lon"]) == string.Empty ? 0.0 : Convert.ToDouble(list["lon"]),
            //                        basic_date = Convert.ToDateTime(list["basic_date"])
            //                    });
            //                }
            //            }
            //        }
            //        this.DataContext = hosplist;
            //        isFavorite = false;
            //        StsResult.Content = $"동물병원 검색결과 {hosplist.Count}건 조회완료";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await Commons.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            //}
        }
        #endregion

        #region < 버튼 함수 부분 >
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 병원을 선택하세요(복수선택 가능)");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 병원입니다");
                return;
            }
            try
            {
                // DB 연결확인
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO FavoriteHosp
                                             (
                                              gugun
                                             ,animal_hospital
                                             ,approval
                                             ,road_address
                                             ,tel
                                             ,lat
                                             ,lon
                                             ,basic_date)
                                       VALUES
                                             (
                                              @gugun 
                                             ,@animal_hospital
                                             ,@approval 
                                             ,@road_address 
                                             ,@tel
                                             ,@lat
                                             ,@lon
                                             ,@basic_date)";

                    var insRes = 0;
                    foreach (FavoriteHosp item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@gugun", item.gugun);
                        cmd.Parameters.AddWithValue("@animal_hospital", item.animal_hospital);
                        cmd.Parameters.AddWithValue("@approval", item.approval);
                        cmd.Parameters.AddWithValue("@road_address", item.road_address);
                        cmd.Parameters.AddWithValue("@tel", item.tel);
                        cmd.Parameters.AddWithValue("@lat", item.lat);
                        cmd.Parameters.AddWithValue("@lon", item.lon);
                        cmd.Parameters.AddWithValue("@basic_date", item.basic_date);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                        StsResult.Content = $"즐겨찾기 {insRes} 건 저장완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요");
                    }
                    //MessageBox.Show(insRes.ToString());
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtHptName.Text = string.Empty;

            List<FavoriteHosp> list = new List<FavoriteHosp>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT 
                                        gugun
                                       ,animal_hospital
                                       ,approval
                                       ,road_address
                                       ,tel
                                       ,lat
                                       ,lon
                                       ,basic_date
                                   FROM FavoriteHosp";
                    var cmd = new SqlCommand(query, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteHosp");

                    foreach (DataRow dr in dSet.Tables["FavoriteHosp"].Rows)
                    {
                        list.Add(new FavoriteHosp
                        {
                            gugun = Convert.ToString(dr["gugun"]),
                            animal_hospital = Convert.ToString(dr["animal_hospital"]),
                            approval = Convert.ToDateTime(dr["approval"]),
                            road_address = Convert.ToString(dr["road_address"]),
                            tel = Convert.ToString(dr["tel"]),
                            lat = Convert.ToDouble(dr["lat"]),
                            lon = Convert.ToDouble(dr["lon"]),
                            basic_date = Convert.ToDateTime(dr["basic_date"])
                        });
                    }

                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
            }
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 병원을 선택하세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM FavoriteHosp WHERE animal_hospital = @animal_hospital";
                    var delRes = 0;

                    foreach (FavoriteHosp item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@animal_hospital", item.animal_hospital);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제성공!!");
                        BtnViewFavorite_Click(sender, e); // 즐겨찾기 보기 이벤트핸들러를 한번 실행
                        StsResult.Content = $"즐겨찾기 {delRes} 건 삭제완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 일부성공!!"); // 발생할일이 거의 전무
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }
        }
        #endregion
    }
}
