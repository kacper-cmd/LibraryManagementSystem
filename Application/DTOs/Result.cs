﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
	public readonly struct Result<T, E>
	{
		private readonly bool _success;
		public readonly T Value;
		public readonly E Error;

		private Result(T v, E e, bool success)
		{
			Value = v;
			Error = e;
			_success = success;
		}

		public bool IsOk => _success;

		public static Result<T, E> Ok(T v)
		{
			return new(v, default(E), true);
		}

		public static Result<T, E> Err(E e)
		{
			return new(default(T), e, false);
		}

		public static implicit operator Result<T, E>(T v) => Ok(v);
		public static implicit operator Result<T, E>(E e) => Err(e);


		public R Match<R>(
				Func<T, R> success,
				Func<E, R> failure) =>
			_success ? success(Value) : failure(Error);
	}
}
