using System;

namespace Neo.IO.Data.LightDB
{
    public class Options
    {
        public static readonly Options Default = new Options();
        internal readonly IntPtr handle = RocksDbSharp.Native.Instance.rocksdb_options_create();

        public bool CreateIfMissing
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_create_if_missing(handle, value);
            }
        }

        public bool ErrorIfExists
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_error_if_exists(handle, value);
            }
        }

        public bool ParanoidChecks
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_paranoid_checks(handle, value);
            }
        }

        public int WriteBufferSize
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_write_buffer_size(handle, (UIntPtr)value);
            }
        }

        public int MaxOpenFiles
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_max_open_files(handle, value);
            }
        }

        public int BlockSize
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_block_based_options_set_block_size(handle, (UIntPtr)value);
            }
        }

        public int BlockRestartInterval
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_block_based_options_set_block_restart_interval(handle, value);
            }
        }

        public RocksDbSharp.CompressionTypeEnum Compression
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_compression(handle, value);
            }
        }

        public IntPtr FilterPolicy
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_block_based_options_set_filter_policy(handle, value);
            }
        }

        ~Options()
        {
            RocksDbSharp.Native.Instance.rocksdb_options_destroy(handle);
        }
    }
}
