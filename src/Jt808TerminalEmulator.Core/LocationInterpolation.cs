using Jt808TerminalEmulator.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jt808TerminalEmulator.Core
{
    /// <summary>
    /// 经纬度插值
    /// </summary>
    public class LocationInterpolation
    {
        readonly static long radius = 6371393; // 地球的平均半径，以km为单位
        readonly static double pi = Math.PI;

        /// <summary>
        /// 辅助功能将度数转换为弧度
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegToRad(double deg)
        {
            return (deg * pi / 180);
        }

        /// <summary>
        /// 辅助功能将弧度转换为度
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double RadToDeg(double rad)
        {
            return (rad * 180 / pi);
        }

        /// <summary>
        /// 计算两点之间的（初始）方位角，以度为单位
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double CalculateBearing(LocationDto startPoint, LocationDto endPoint)
        {
            var lat1 = DegToRad(startPoint.Latitude);
            var lat2 = DegToRad(endPoint.Latitude);
            var deltaLon = DegToRad(endPoint.Logintude - startPoint.Logintude);

            var y = Math.Sin(deltaLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);
            var bearing = Math.Atan2(y, x);

            // 由于atan2返回的值介于-180和+180之间，因此我们需要将其转换为0-360度
            return (RadToDeg(bearing) + 360) % 360;
        }

        /// <summary>
        /// 在给定的初始方位上，从已经走了给定距离（以km为单位）的给定点计算出目的地点（在到达目的地之前，轴承可能会有所不同）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="bearing"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static LocationDto CalculateDestinationLocation(LocationDto point, double bearing, double distance)
        {

            distance /= radius; // 转换为弧度的角距离
            bearing = DegToRad(bearing); // 将方位角转换为弧度

            var lat1 = DegToRad(point.Latitude);
            var lon1 = DegToRad(point.Logintude);

            var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance) + Math.Cos(lat1) * Math.Sin(distance) * Math.Cos(bearing));
            var lon2 = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance) * Math.Cos(lat1), Math.Cos(distance) - Math.Sin(lat1) * Math.Sin(lat2));
            lon2 = (lon2 + 3 * pi) % (2 * pi) - pi; // 标准化为-180 <-> +180度

            return new LocationDto { Latitude = RadToDeg(lat2), Logintude = RadToDeg(lon2) };
        }

        /// <summary>
        /// 计算两点之间的距离（以km为单位）
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double CalculateDistanceBetweenLocations(LocationDto startPoint, LocationDto endPoint)
        {

            var lat1 = DegToRad(startPoint.Latitude);
            var lon1 = DegToRad(startPoint.Logintude);

            var lat2 = DegToRad(endPoint.Latitude);
            var lon2 = DegToRad(endPoint.Logintude);

            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (radius * c);
        }

        /// <summary>
        /// 给定行进距离（以km为单位），现在可以计算两点之间的中间位置
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="endLocation"></param>
        /// <param name="distanceTravelled">离起点的距离</param>
        /// <returns></returns>
        public static LocationDto IntermediaryLocation(LocationDto startLocation, LocationDto endLocation, double distanceTravelled)
        {
            var bearing = CalculateBearing(startLocation, endLocation);
            return CalculateDestinationLocation(startLocation, bearing, distanceTravelled);
        }

        /// <summary>
        /// 根据两个点以、速度、持续时间获取每秒所产生的点
        /// </summary>
        /// <param name="startLocation">起点</param>
        /// <param name="endLocation">终端</param>
        /// <param name="speed">速度</param>
        /// <param name="duration">持续时间，单位秒</param>
        /// <returns></returns>
        public static List<LocationDto> IntermediaryLocation(LocationDto startLocation, LocationDto endLocation, double speed, long duration)
        {
            var locations = new List<LocationDto>();

            for (int i = 0; i < duration; i++)
            {
                double bearing = CalculateBearing(startLocation, endLocation);
                double distanceInKm = speed / 1000;
                var intermediaryLocation = CalculateDestinationLocation(startLocation, bearing, distanceInKm);

                locations.Add(intermediaryLocation);

                // 将中介位置设置为新的起始位置
                startLocation = intermediaryLocation;
            }
            return locations;
        }

        public static List<LocationDto> Reinterpolation(LineDto dto, out double alldistance)
        {
            var intervalDistance = dto.Speed / 3600 * dto.Interval;
            alldistance = 0d;
            var locations = new List<LocationDto>
            {
                dto.Locations.First()
            };
            var nextDistance = intervalDistance;
            var startLocation = locations.Last();
            for (int i = 1; i < dto.Locations.Count - 1; i++)
            {
                var endLocation = dto.Locations[i];
                alldistance += LocationInterpolation.CalculateDistanceBetweenLocations(dto.Locations[i - 1], endLocation);
                while (true)
                {
                    var distance = LocationInterpolation.CalculateDistanceBetweenLocations(startLocation, endLocation);
                    if (distance < nextDistance)
                    {
                        nextDistance -= distance;
                        startLocation = endLocation;
                        break;
                    }
                    nextDistance = intervalDistance;
                    startLocation = LocationInterpolation.IntermediaryLocation(startLocation, endLocation, intervalDistance);
                    locations.Add(startLocation);
                }
            }
            return locations;
        }
    }
}
