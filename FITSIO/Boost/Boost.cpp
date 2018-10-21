// Boost.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Boost.h"

namespace NajmBoost
{
	extern "C" BOOST_API void FixByteOrder(BYTE *pData, __int64 nNumElements, __int64 nElementSize)
	{
		BYTE *pDataMax = pData + nNumElements;
		while(pData < pDataMax)
		{
			for (int j = 0; j < nElementSize / 2 ; j++)
            {
                BYTE b = pData[j];
                pData[j] = pData[nElementSize - 1 - j];
                pData[nElementSize - 1 - j] = b;
            }
			pData += nElementSize;
		}
	}

	template<typename T> void UnifyDataCore(double *pdblData, __int64 nNumElements, BYTE *pbData)
	{
		T* pData = (T*)pbData;
		for(__int64 i=0 ; i < nNumElements ; i++)
		{
			*pdblData++ = (double)*pData++;
		}
	}

	extern "C" BOOST_API void UnifyData(double *pdblData, __int64 nNumElements, BYTE *pbData, __int64 nNumBytes, __int64 nOffset, int bitsPerPixel)
	{
		switch(bitsPerPixel)
		{
		case 8:
			UnifyDataCore<BYTE>(pdblData, nNumElements, pbData + nOffset);
			break;
		case 16:
			UnifyDataCore<short>(pdblData, nNumElements, pbData + nOffset);
			break;
		case 32:
			UnifyDataCore<int>(pdblData, nNumElements, pbData + nOffset);
			break;
		case -32:
			UnifyDataCore<float>(pdblData, nNumElements, pbData + nOffset);
			break;
		}
	}
}
