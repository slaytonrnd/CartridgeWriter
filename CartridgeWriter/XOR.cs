// Derived from xor.c by David Slayton (2014); acknowledgment below.

/*
 *  xor.c : Source for the trivial cipher which XORs the message with the key.
 *          The key can be up to 32 bytes long.
 *
 * Part of the Python Cryptography Toolkit
 *
 * Contributed by Barry Warsaw and others.
 *
 * =======================================================================
 * The contents of this file are dedicated to the public domain.  To the
 * extent that dedication to the public domain is not available, everyone
 * is granted a worldwide, perpetual, royalty-free, non-exclusive license
 * to exercise all rights associated with the contents of this file for
 * any purpose whatsoever.  No rights are reserved.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * =======================================================================
 */

using System;

namespace CartridgeWriter
{
    public class XOR
    {
        private const int MAX_KEY_SIZE = 32;
        private int keyLength;
        private int lastPosition = 0;

        public byte[] Key { get; private set; }
        
        public XOR(byte[] key)
        {
            if (key == null) throw new ArgumentNullException("key");

            keyLength = key.Length;

            if (keyLength > MAX_KEY_SIZE || keyLength <= 0)
                throw new ArgumentOutOfRangeException("key length must be between 1 byte and 32 bytes");

            Key = new byte[keyLength];
            Buffer.BlockCopy(key, 0, Key, 0, keyLength);
        }

        public byte[] Crypt(byte[] block)
        {
            if (block == null) throw new ArgumentNullException("block");

            int len = block.Length;

            if (len < 1) throw new ArgumentOutOfRangeException("block length must be at least 1 byte");

            int j = lastPosition;
            byte[] ret = new byte[len];

            for (int i = 0; i < len; i++)
            {
                ret[i] = (byte)(block[i] ^ Key[j]);
                j = (j + 1) % keyLength;
            }

            lastPosition = j;

            return ret;
        }
    }
}
