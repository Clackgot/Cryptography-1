﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography.Ciphers
{
    class Wheatstone_Double_Square
    {
		private static string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";//Набор сиволов
		private string NewAlphabet = Alphabet;//Не статическая копия алфавита

		private string text = "";//Входной текст

		private int[] firstNumberKey;//Первый числовой ключ
		private int[] secondNumberKey;//Второй числовой ключ

		private string encrypt;//Зашифрованные данные
		private string decrypt;//Дешифрованные данные

		private int height;//Высота таблицы
		private int width;//Широта таблицы

		private char[,] Table;//Таблица
		private char[,] FirstTable;//Таблица смены столбцов
		private char[,] SecondTable;//Таблица смены строк

		/// <summary>
		/// 
		/// </summary>
		/// <param name="firstKey"></param>
		/// <param name="secondKey"></param>
		/// <param name="text"></param>
		/// <param name="Height"></param>
		/// <param name="Alphabet"></param>
		public Wheatstone_Double_Square(string firstKey, string secondKey, string text, out int Height, out string Alphabet)
		{
			this.text = text;//Исходный текст
			this.firstNumberKey = ConvertToNumber(firstKey);//Первый ключ
			this.secondNumberKey = ConvertToNumber(secondKey);//Второй ключ
			this.width = firstKey.Length;//Ширина таблицы
			this.height = (int)Math.Ceiling(Convert.ToDouble(text.Length) / Convert.ToDouble(width));//Высота таблицы
			this.Table = new char[height, width];//Исходная Таблица
			this.FirstTable = new char[height, width];//Первое действие шифрования таблицы по столбцам
			this.SecondTable = new char[height, width];//Второе действие шифрование таблицы по строкам
			Height = height;//Возвращаем высоту таблицы
			Alphabet = NewAlphabet;//Возвращаем алфавит
		}

		//Проверка ключа на число и запись в численный массив
		private int[] ConvertToNumber(string key)
		{
			int[] NumberKey = new int[key.Length];
			bool IsNumber = int.TryParse(key.Trim(), out int number);
			if (IsNumber)//Для чисел
			{
				string str = number.ToString();
				for (int i = 0; i < key.Length; i++)
				{
					NumberKey[i] = Convert.ToInt32(str[i]);
				}
			}
			else//Для слов - привет -> 16 17 9 2 5 19
			{
				for (int i = 0; i < key.Length; i++)//Перебираем все символы ключа
				{
					NumberKey[i] = Alphabet.IndexOf(key[i]);
				}
			}
			int[] NewNumberKey = (int[])NumberKey.Clone();
			Array.Sort(NewNumberKey);

			for (int i = 0; i < NumberKey.Length; i++)//Упрощаем численную запись 16 17 9 2 5 19 -> 3 4 2 0 1 5
			{
				NumberKey[i] = Array.IndexOf(NewNumberKey, NumberKey[i]);//Прибавить 1 для 12345
			}
			return NumberKey;
		}

		/// <summary>
		/// Возвращает засшифрованную строку
		/// Return encode string
		/// </summary>
		/// <returns></returns>
		public string Encode()
		{
			for (int i = 0, n = 0; i < height; i++)//Заполнение Таблицы исходным текстом
			{
				for (int j = 0; j < width; j++)
				{
					Table[i, j] = (text.Length == n) ? ' ' : text[n++];
				}
			}
			for (int i = 0; i < height; i++)//Меняем столбцы местами
			{
				for (int j = 0; j < width; j++)
				{
					FirstTable[i, j] = Table[i, Array.IndexOf(firstNumberKey, j)];
				}
			}
			for (int i = 0; i < height; i++)//Меняем строки местами
			{
				for (int j = 0; j < width; j++)
				{
					SecondTable[i, j] = FirstTable[Array.IndexOf(secondNumberKey, i), j];
				}
			}
			for (int i = 0; i < width; i++)//Преобразуем таблицу в строку шифра
			{
				for (int j = 0; j < height; j++)
				{
					encrypt += SecondTable[j, i];
				}
			}
			return encrypt;
		}

		/// <summary>
		/// Возвращает расшифрованную строку
		/// Return decode string 
		/// </summary>
		/// <returns></returns>
		public string Decode()
		{
			for (int i = 0, n = 0; i < width; i++)//Запихиваем строку шифра в таблицу обратно
			{
				for (int j = 0; j < height; j++)
				{
					Table[j, i] = (text.Length == n) ? ' ' : text[n++];
				}
			}
			for (int i = 0; i < height; i++)//Меняем строки местами
			{
				for (int j = 0; j < width; j++)
				{
					FirstTable[Array.IndexOf(secondNumberKey, i), j] = Table[i, j];
				}
			}
			for (int i = 0; i < height; i++)//Меняем столбцы местами
			{
				for (int j = 0; j < width; j++)
				{
					SecondTable[i, Array.IndexOf(firstNumberKey, j)] = FirstTable[i, j];
				}
			}
			for (int i = 0; i < height; i++)//Преобразуем таблицу в строку
			{
				for (int j = 0; j < width; j++)
				{
					decrypt += SecondTable[i, j];
				}
			}
			return decrypt;
		}
	}
}
