// #region License
//
// /*
// Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
// You may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.
// */
//
// #endregion
//
// using System;
// namespace ChilliSource.Mobile.Core
// {
// 	/// <summary>
// 	/// Functional extensions for handling the various outcome 
// 	/// states of <see cref="T:ChilliSource.Mobile.Core.ServiceResult"/>
// 	/// </summary>
// 	public static class ServiceResultExtensions
// 	{
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> is successful
//         /// </summary>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
//         public static ServiceResult OnSuccess(this ServiceResult result, Func<ServiceResult, ServiceResult> action)
// 		{
// 			return result.IsSuccessful ? action(result) : result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> has failed
//         /// </summary>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
//         public static ServiceResult OnFailure(this ServiceResult result, Action<ServiceResult> action)
// 		{
// 			if (result.IsFailure)
// 			{
// 				action(result);
// 			}
//
// 			return result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> was cancelled
//         /// </summary>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
//         /// <returns></returns>
// 		public static ServiceResult OnCancelled(this ServiceResult result, Action<ServiceResult> action)
// 		{
// 			if (result.IsCancelled)
// 			{
// 				action(result);
// 			}
//
// 			return result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> once the result has completed, regardless of its outcome
//         /// </summary>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
//         public static ServiceResult Always(this ServiceResult result, Action<ServiceResult> action)
// 		{
// 			action(result);
// 			return result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> is successful
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
//         public static ServiceResult<T> OnSuccess<T>(this ServiceResult<T> result, Func<ServiceResult<T>, ServiceResult<T>> action)
// 		{
// 			return result.IsSuccessful ? action(result) : result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> has failed
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
// 		public static ServiceResult<T> OnFailure<T>(this ServiceResult<T> result, Action<ServiceResult<T>> action)
// 		{
// 			if (result.IsFailure)
// 			{
// 				action(result);
// 			}
//
// 			return result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> if the <paramref name="result"/> was cancelled
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
// 		public static ServiceResult<T> OnCancelled<T>(this ServiceResult<T> result, Action<ServiceResult<T>> action)
// 		{
// 			if (result.IsCancelled)
// 			{
// 				action(result);
// 			}
//
// 			return result;
// 		}
//
//         /// <summary>
//         /// Executes <paramref name="action"/> once the result has completed, regardless of its outcome
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <param name="result"></param>
//         /// <param name="action"></param>
//         /// <returns>The <paramref name="result"/></returns>
// 		public static ServiceResult<T> Always<T>(this ServiceResult<T> result, Action<ServiceResult<T>> action)
// 		{
// 			action(result);
// 			return result;
// 		}
// 	}
// }
